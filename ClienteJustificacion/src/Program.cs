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
        private static Random random = new Random();
        private static Logger _loger = LogManager.GetCurrentClassLogger();
        private static string user;
        private static string password;

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


        static void ProcessJbs(ClientArgs clientArgs)
        {
            var records = LoadCsv<JbsInterchageModel, JbsInterchageModelCsvDocMap>(clientArgs.File);
            var result = SendRecords(records.Cast<JInterchageModel>().ToList());
        }

        static void ProcessJviajes(ClientArgs clientArgs)
        {
            var records = LoadCsv<JviajeInterchageModel, JviajesInterchageModelCsvMap>(clientArgs.File);
            var result = SendRecords(records.Cast<JInterchageModel>().ToList());
        }

        static void ProcessJpersonal(ClientArgs clientArgs)
        {
            var records = LoadCsv<JpersonalInterchageModel, JpersonalInterchageModelCsvMap>(clientArgs.File);
            var result = SendRecords(records.Cast<JInterchageModel>().ToList());
        }

        /// <summary>
        /// Procesa el fichero CSV de entrada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        static List<T> LoadCsv<T, U>(string path) where T : JInterchageModel where U : CsvClassMap
        {
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
        }

        /// <summary>
        /// Envio de datos al WS
        /// </summary>
        /// <param name="records"></param>
        /// <returns></returns>
        static List<LoadResult> SendRecords(List<JInterchageModel> records)
        {
            var resultadosEnvio = new List<LoadResult>();

            using (JustificationClient client = new JustificationClient())
            {
                client.ClientCredentials.UserName.UserName = user;
                client.ClientCredentials.UserName.Password = password;
                foreach (var item in records.Select((value, i) => new { i, value }))
                {
                    Console.Write("Envio: {0}, {1}.", item.i, item.value.Expediente);
                    if (!string.IsNullOrWhiteSpace(item.value.NombreFichero))
                    {
                        item.value.Fichero = File.ReadAllBytes(item.value.NombreFichero);
                        item.value.NombreFichero = Path.GetFileName(item.value.NombreFichero);
                    }

                    LoadResult r;
                    if (item.value is JbsInterchageModel)
                    {
                        var j = (JbsInterchageModel)item.value;
                        r = client.LoadJbs(j);
                        resultadosEnvio.Add(r);
                    }
                    else if (item.value is JpersonalInterchageModel)
                    {
                        var j = (JpersonalInterchageModel)item.value;
                        r = client.LoadJpersonal(j);
                        resultadosEnvio.Add(r);
                    }
                    else if (item.value is JviajeInterchageModel)
                    {
                        var j = (JviajeInterchageModel)item.value;
                        r = client.LoadJviajes(j);
                        resultadosEnvio.Add(r);
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
                        }
                    }
                }
            }

            return resultadosEnvio;
        }

    }

}
