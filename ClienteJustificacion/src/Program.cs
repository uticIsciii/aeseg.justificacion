using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using CsvHelper;
using PowerArgs;
using System.IO;
using Aeseg.ProxyJustificacion;
using CsvHelper.Configuration;

namespace ClienteJustificacion
{
    class Program
    {
        #region PROPIEDADES

        private static Random random = new Random();
        private static Logger _loger = LogManager.GetCurrentClassLogger();
        private static string user;
        private static string password;

        #endregion

        #region EJECUCIÓN DEL PROCESO INICIAL

        static void Main(string[] args)
        {
            try
            {
                var clientArgs = Args.Parse<ClientArgs>(args);
                Console.WriteLine("Proceso del fichero: {0}; {1}; {2}; {3};", clientArgs.User, "*****", clientArgs.FileType, clientArgs.File);
                _loger.Info("Proceso del fichero: {0}; {1}; {2}; {3};", clientArgs.User, "*****", clientArgs.FileType, clientArgs.File);
                user = clientArgs.User;
                password = clientArgs.Password;

                switch (clientArgs.FileType)
                {
                    case JustificacionFileType.Bienes:
                        ProcessJbs(clientArgs);
                        break;
                    case JustificacionFileType.Viajes:
                        ProcessJviajes(clientArgs);
                        break;
                    case JustificacionFileType.Personal:
                        ProcessJpersonal(clientArgs);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (ArgException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<ClientArgs>());
            }
            catch (Exception e)
            {
                _loger.Error(e, "error!");
                Console.WriteLine("Error." + e.Message);
            }
        }

        #endregion

        #region MÉTODOS PRINCIPALES DE PROCESOS DE OBTENCIÓN Y ENVIO DE DATOS DE JUSTIFICANTES (BIENES Y SERVICIOS, PERSONAL Y VIAJE) AL SERVICIO WEB JUSTIFICACIÓN

        static void ProcessJbs(ClientArgs clientArgs)
        {
            var records = LoadCsv<JbsInterchageModel, JbsInterchageModelCsvDocMap>(clientArgs.File);
            var result = SendRecords(records.Cast<JInterchageModel>().ToList());
        }

        static void ProcessJpersonal(ClientArgs clientArgs)
        {
            var records = LoadCsv<JpersonalInterchageModel, JpersonalInterchageModelCsvMap>(clientArgs.File);
            var result = SendRecords(records.Cast<JInterchageModel>().ToList());
        }

        static void ProcessJviajes(ClientArgs clientArgs)
        {
            var records = LoadCsv<JviajeInterchageModel, JviajesInterchageModelCsvMap>(clientArgs.File);
            var result = SendRecords(records.Cast<JInterchageModel>().ToList());
        }

        #endregion

        #region MÉTODO DE OBTENCIÓN DE DATOS DE JUSTIFICANTES
        
        /// <summary>
        /// Procesa el fichero CSV de entrada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        static List<T> LoadCsv<T, U>(string path) where T : JInterchageModel where U : CsvClassMap
        {
            #region OBTENCIÓN DE DATOS DEL CSV (BIENES Y SERVICIOS, PERSONAL, O VIAJE)
          
            //Carga y parseo del fichero de datos jbs
            var parseErrors = new List<ParseResult>(); //Listado de errores de parseo
            var csv = new CsvReader(new StreamReader(path));
            csv.Configuration.Delimiter = ";";
            csv.Configuration.RegisterClassMap<U>();
            csv.Configuration.IgnoreReadingExceptions = true;
            csv.Configuration.ReadingExceptionCallback = (ex, row) =>
            {
                int rownumber = row.Row;
                string message = ex.Message + " " + ex.Data["CsvHelper"];
                parseErrors.Add(new ParseResult() { Id = rownumber, Descripcion = message });
            };
            var records = csv.GetRecords<T>().ToList();

            if (parseErrors.Count > 0)
            {
                Console.WriteLine("Se ha encontrado errores en el fichero de entrada, corrijalos antes de continuar.");
                _loger.Error("Se ha encontrado errores en el fichero de entrada, corrijalos antes de continuar.");
                foreach (var error in parseErrors)
                {
                    Console.WriteLine("Linea: {0}; Error {1};", error.Id, error.Descripcion);
                    _loger.Error("Linea: {0}; Error {1};", error.Id, error.Descripcion);
                }
                throw new ArgumentException("Formato del fichero de entrada no valido.");
            }
            return records;
            
            #endregion
        }
            
        #endregion

        #region MÉTODO DE ENVIO DE DATOS DE JUSTIFICANTES

        /// <summary>
        /// Envio de datos al WS - JUSTIFICACIÓN
        /// </summary>
        /// <param name="records">Listado que contiene los valores de modelo de datos de Justificación del Servicio Web</param>
        /// <returns></returns>
        static List<LoadResult> SendRecords(List<JInterchageModel> records)
        {
            #region ENVÍO DE DATOS A MÉTODOS DEL SERVICIO WEB JUSTIFICACIÓN

            var resultadosEnvio = new List<LoadResult>();

            using (JustificationClient client = new JustificationClient())
            {
               


                client.ClientCredentials.UserName.UserName = user;
                client.ClientCredentials.UserName.Password = password;

                foreach (var item in records.Select((value, i) => new { i, value }))
                {
                    Console.Write("Envio: {0}, {1}.", item.i, item.value.Expediente);
    
                    LoadResult r;
                    if (item.value is JbsInterchageModel)
                    {
                        #region Llamada al Servicio Web de Justificación Bienes y Servicios
                        var j = (JbsInterchageModel)item.value;
                        //Carga de documentos múltiples en modelo Justificante de Bienes y Servicios
                        j.FicheroBSFactura = File.ReadAllBytes(j.NombreFicheroBSFactura); //obtenemos los byte[] del fichero a través de la ruta que venga en el csv
                        j.NombreFicheroBSFactura = Path.GetFileName(j.NombreFicheroBSFactura); //obtenemos el nombre del fichero a través de la ruta que venga en el csv

                        j.FicheroBSPago = File.ReadAllBytes(j.NombreFicheroBSPago); //obtenemos los byte[] del fichero a través de la ruta que venga en el csv
                        j.NombreFicheroBSPago = Path.GetFileName(j.NombreFicheroBSPago); //obtenemos el nombre del fichero a través de la ruta que venga en el csv

                        j.FicheroBSOtros = File.ReadAllBytes(j.NombreFicheroBSOtros); //obtenemos los byte[] del fichero a través de la ruta que venga en el csv
                        j.NombreFicheroBSOtros = Path.GetFileName(j.NombreFicheroBSOtros); //obtenemos el nombre del fichero a través de la ruta que venga en el csv

                        r = client.LoadJbs(j); //Llamada al método LoadJbs del servicio web justificación 

                        resultadosEnvio.Add(r);
                        #endregion
                    }
                    else if (item.value is JpersonalInterchageModel)
                    {
                        #region Llamada al Servicio Web de Justificación Personal
                        var j = (JpersonalInterchageModel)item.value;
                        //Carga de documentos múltiples en modelo Justificante de Personal
                        j.FicheroPerNomina = File.ReadAllBytes(j.NombreFicheroPerNomina);
                        j.NombreFicheroPerNomina = Path.GetFileName(j.NombreFicheroPerNomina);

                        j.FicheroPerPago = File.ReadAllBytes(j.NombreFicheroPerPago);
                        j.NombreFicheroPerPago = Path.GetFileName(j.NombreFicheroPerPago);

                        j.FicheroPerOtros = File.ReadAllBytes(j.NombreFicheroPerOtros);
                        j.NombreFicheroPerOtros = Path.GetFileName(j.NombreFicheroPerOtros);

                        r = client.LoadJpersonal(j); //Llamada al método LoadJpersonal del servicio web justificación 

                        resultadosEnvio.Add(r);
                        #endregion
                    }
                    else if (item.value is JviajeInterchageModel)
                    {
                        #region Llamada al Servicio Web de Justificación Viaje
                        var j = (JviajeInterchageModel)item.value;
                        //Carga de documentos múltiples en modelo Justificante de Viaje
                        j.FicheroViajeFactura = File.ReadAllBytes(j.NombreFicheroViajeFactura);
                        j.NombreFicheroViajeFactura = Path.GetFileName(j.NombreFicheroViajeFactura);

                        j.FicheroViajePago = File.ReadAllBytes(j.NombreFicheroViajePago);
                        j.NombreFicheroViajePago = Path.GetFileName(j.NombreFicheroViajePago);

                        j.FicheroViajeOtros = File.ReadAllBytes(j.NombreFicheroViajeOtros);
                        j.NombreFicheroViajeOtros = Path.GetFileName(j.NombreFicheroViajeOtros);

                        r = client.LoadJviajes(j); //Llamada al método LoadJviajes del servicio web justificación 

                        resultadosEnvio.Add(r);
                        #endregion
                    }
                    else
                    {
                        throw new ArgumentException("Tipo de datos incorrecto.");
                    }
                    Console.WriteLine(" Resultado: {0};", r.ResultadoCarga);
                    _loger.Info("Envio: {0}, {1}. Resultado {2}", item.i, item.value.Expediente, r.ResultadoCarga);
                    if (r.ResultadoCarga == ResultadoCarga.Erroneo)
                    {
                        foreach (var error in r.Errores)
                        {
                            _loger.Warn("Envio: {0}, {1}. Error {2} - {3}", item.i, item.value.Expediente, error.Nombre, error.Descripcion);
                            Console.WriteLine("Envio: {0}, {1}. Error {2} - {3}", item.i, item.value.Expediente, error.Nombre, error.Descripcion);

                        }
                    }
                }
            }

            return resultadosEnvio;
            
            #endregion
        }

        #endregion
    }

}
