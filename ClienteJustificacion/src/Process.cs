using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using ISCIII.AESEG.ClienteJustificacion.Proxy.ProxyJustificacion;
using NLog;

namespace ISCIII.AESEG.ClienteJustificacion.BLL
{
    public class Process
    {
        #region PROPIEDADES

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion PROPIEDADES

        #region MÉTODOS PRINCIPALES DE PROCESOS DE OBTENCIÓN Y ENVIO DE DATOS DE JUSTIFICANTES (BIENES Y SERVICIOS, PERSONAL Y VIAJE) AL SERVICIO WEB JUSTIFICACIÓN

        public static void ProcesarJustificantesBienesServicios(ClientArgs clientArgs)
        {
            var records = CargarCsv<JbsInterchageModel, JbsInterchageModelCsvDocMap>(clientArgs.File, clientArgs.Encoding);
            _logger.Info("Cargado fichero {0}", clientArgs.File);
            var result = SendRecords(records.Cast<JInterchageModel>().ToList(), clientArgs.User, clientArgs.Password);
        }

        public static void ProcesarJustificantesPersonal(ClientArgs clientArgs)
        {
            var records = CargarCsv<JpersonalInterchageModel, JpersonalInterchageModelCsvMap>(clientArgs.File, clientArgs.Encoding);
            _logger.Info("Cargado fichero {0}", clientArgs.File);
            var result = SendRecords(records.Cast<JInterchageModel>().ToList(), clientArgs.User, clientArgs.Password);
        }

        public static void ProcesarJustificantesViajes(ClientArgs clientArgs)
        {
            var records = CargarCsv<JviajeInterchageModel, JviajesInterchageModelCsvMap>(clientArgs.File, clientArgs.Encoding);
            _logger.Info("Cargado fichero {0}", clientArgs.File);
            var result = SendRecords(records.Cast<JInterchageModel>().ToList(), clientArgs.User, clientArgs.Password);
        }

        #endregion MÉTODOS PRINCIPALES DE PROCESOS DE OBTENCIÓN Y ENVIO DE DATOS DE JUSTIFICANTES (BIENES Y SERVICIOS, PERSONAL Y VIAJE) AL SERVICIO WEB JUSTIFICACIÓN

        #region MÉTODO DE OBTENCIÓN DE DATOS DE JUSTIFICANTES

        /// <summary>
        /// Procesa el fichero CSV de entrada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        private static List<T> CargarCsv<T, U>(string path, Codificacion codificacion) where T : JInterchageModel where U : CsvClassMap
        {
            #region OBTENCIÓN DE DATOS DEL CSV (BIENES Y SERVICIOS, PERSONAL, O VIAJE)

            //Carga y parseo del fichero de datos jbs
            var parseErrors = new List<ParseResult>(); //Listado de errores de parseo
            Encoding encoding = GetEncoding(codificacion);

            var csv = new CsvReader(new StreamReader(path, encoding));
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

            if (parseErrors.Any())
            {
                _logger.Warn("Se han encontrado errores en el fichero de entrada, por favor, corríjalos antes de continuar.");
                foreach (var error in parseErrors)
                {
                    _logger.Warn("Error en la línea {0} --> {1};", error.Id, error.Descripcion);
                }
                throw new ArgumentException("El formato del fichero de entrada no es válido. Se ha cancelado la operación.");
            }
            return records;

            #endregion OBTENCIÓN DE DATOS DEL CSV (BIENES Y SERVICIOS, PERSONAL, O VIAJE)
        }

        private static Encoding GetEncoding(Codificacion codificacion)
        {
            switch (codificacion)
            {
                case Codificacion.Windows1252:
                    return Encoding.GetEncoding("Windows-1252");
                case Codificacion.UTF8:
                default:
                    return Encoding.UTF8;
            }
        }

        #endregion MÉTODO DE OBTENCIÓN DE DATOS DE JUSTIFICANTES

        #region MÉTODO DE ENVIO DE DATOS DE JUSTIFICANTES

        /// <summary>
        /// Envio de datos al WS - JUSTIFICACIÓN
        /// </summary>
        /// <param name="records">Listado que contiene los valores de modelo de datos de Justificación del Servicio Web</param>
        /// <returns></returns>
        private static List<LoadResult> SendRecords(List<JInterchageModel> records, string usuario, string password)
        {
            #region ENVÍO DE DATOS A MÉTODOS DEL SERVICIO WEB JUSTIFICACIÓN

            var resultadosEnvio = new List<LoadResult>();

            var client = new JustificationClient();

            #region Descomentar código para ignorar certificados

            //System.Net.ServicePointManager.ServerCertificateValidationCallback =
            //delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            //{ return true; };

            #endregion

            try
            {
                client.ClientCredentials.UserName.UserName = usuario;
                client.ClientCredentials.UserName.Password = password;
                client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

                int total = records.Count;

                foreach (var item in records.Select((value, i) => new { i, value }))
                {
                    LoadResult r;
                    if (item.value is JbsInterchageModel)
                    {
                        #region Llamada al Servicio Web de Justificación Bienes y Servicios

                        var j = (JbsInterchageModel)item.value;
                        //Carga de documentos múltiples en modelo Justificante de Bienes y Servicios
                        if (!string.IsNullOrEmpty(j.NombreFicheroBSFactura))
                        {
                            j.FicheroBSFactura = File.ReadAllBytes(j.NombreFicheroBSFactura); //obtenemos los byte[] del fichero a través de la ruta que venga en el csv
                            j.NombreFicheroBSFactura = Path.GetFileName(j.NombreFicheroBSFactura); //obtenemos el nombre del fichero a través de la ruta que venga en el csv
                        }
                        if (!string.IsNullOrEmpty(j.NombreFicheroBSPago))
                        {
                            j.FicheroBSPago = File.ReadAllBytes(j.NombreFicheroBSPago); //obtenemos los byte[] del fichero a través de la ruta que venga en el csv
                            j.NombreFicheroBSPago = Path.GetFileName(j.NombreFicheroBSPago); //obtenemos el nombre del fichero a través de la ruta que venga en el csv
                        }
                        if (!string.IsNullOrEmpty(j.NombreFicheroBSOtros))
                        {
                            j.FicheroBSOtros = File.ReadAllBytes(j.NombreFicheroBSOtros); //obtenemos los byte[] del fichero a través de la ruta que venga en el csv
                            j.NombreFicheroBSOtros = Path.GetFileName(j.NombreFicheroBSOtros); //obtenemos el nombre del fichero a través de la ruta que venga en el csv
                        }
                        _logger.Info("Cargando justificante {0} de {1}, Expediente {2}.", item.i + 1, total, item.value.Expediente);

                        r = client.LoadJbs(j); //Llamada al método LoadJbs del servicio web justificación

                        resultadosEnvio.Add(r);

                        #endregion Llamada al Servicio Web de Justificación Bienes y Servicios
                    }
                    else if (item.value is JpersonalInterchageModel)
                    {
                        #region Llamada al Servicio Web de Justificación Personal

                        var j = (JpersonalInterchageModel)item.value;
                        //Carga de documentos múltiples en modelo Justificante de Personal
                        if (!string.IsNullOrEmpty(j.NombreFicheroPerNomina))
                        {
                            j.FicheroPerNomina = File.ReadAllBytes(j.NombreFicheroPerNomina);
                            j.NombreFicheroPerNomina = Path.GetFileName(j.NombreFicheroPerNomina);
                        }

                        if (!string.IsNullOrEmpty(j.NombreFicheroPerPago))
                        {
                            j.FicheroPerPago = File.ReadAllBytes(j.NombreFicheroPerPago);
                            j.NombreFicheroPerPago = Path.GetFileName(j.NombreFicheroPerPago);
                        }

                        if (!string.IsNullOrEmpty(j.NombreFicheroPerOtros))
                        {
                            j.FicheroPerOtros = File.ReadAllBytes(j.NombreFicheroPerOtros);
                            j.NombreFicheroPerOtros = Path.GetFileName(j.NombreFicheroPerOtros);
                        }
                        r = client.LoadJpersonal(j); //Llamada al método LoadJpersonal del servicio web justificación

                        resultadosEnvio.Add(r);

                        #endregion Llamada al Servicio Web de Justificación Personal
                    }
                    else if (item.value is JviajeInterchageModel)
                    {
                        #region Llamada al Servicio Web de Justificación Viaje

                        var j = (JviajeInterchageModel)item.value;
                        //Carga de documentos múltiples en modelo Justificante de Viaje
                        if (!string.IsNullOrEmpty(j.NombreFicheroViajeFactura))
                        {
                            j.FicheroViajeFactura = File.ReadAllBytes(j.NombreFicheroViajeFactura);
                            j.NombreFicheroViajeFactura = Path.GetFileName(j.NombreFicheroViajeFactura);
                        }

                        if (!string.IsNullOrEmpty(j.NombreFicheroViajePago))
                        {
                            j.FicheroViajePago = File.ReadAllBytes(j.NombreFicheroViajePago);
                            j.NombreFicheroViajePago = Path.GetFileName(j.NombreFicheroViajePago);
                        }

                        if (!string.IsNullOrEmpty(j.NombreFicheroViajeOtros))
                        {
                            j.FicheroViajeOtros = File.ReadAllBytes(j.NombreFicheroViajeOtros);
                            j.NombreFicheroViajeOtros = Path.GetFileName(j.NombreFicheroViajeOtros);
                        }

                        r = client.LoadJviajes(j); //Llamada al método LoadJviajes del servicio web justificación

                        resultadosEnvio.Add(r);

                        #endregion Llamada al Servicio Web de Justificación Viaje
                    }
                    else
                    {
                        throw new ArgumentException("Tipo de datos incorrecto.");
                    }

                    _logger.Info("Carga de justificante {0} de {1}, Expediente: {2}. Resultado {3} - {4}", item.i + 1, total, item.value.Expediente, r.ResultadoCarga, r.DescripcionResultado);
                    if (r.ResultadoCarga == ResultadoCarga.Erroneo)
                    {
                        foreach (var error in r.Errores)
                        {
                            _logger.Warn("Carga de justificante {0} de {1}, Expediente: {2}. Error: {3} - {4}", item.i + 1, total, item.value.Expediente, error.Nombre, error.Descripcion);
                        }
                    }
                    _logger.Info("Terminado.");
                }

                return resultadosEnvio;
            }
            // Esta excepción se puede generar en distintos escenarios, por ej: si el usuario/contraseña no son correctos
            // o si el tipo de autenticación del cliente y del servidor no coinciden
            catch (MessageSecurityException e)
            {
                _logger.Warn("Ocurrió un error al autenticar al usuario");
                _logger.Warn(e);
                if (e.InnerException != null)
                {
                    _logger.Warn(e.InnerException.Message);
                }
                throw;
            }

            #endregion ENVÍO DE DATOS A MÉTODOS DEL SERVICIO WEB JUSTIFICACIÓN
        }

        #endregion MÉTODO DE ENVIO DE DATOS DE JUSTIFICANTES
    }
}