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
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static void ProcesarJustificantesBienesServicios(ClientArgs clientArgs)
        {
            var records = CargarCsv<JbsInterchageModel, JbsInterchageModelCsvDocMap>(clientArgs.File, clientArgs.Encoding);
            _logger.Info("Cargado fichero {0}", clientArgs.File);
            SendRecords(records.Cast<JInterchageModel>().ToList(), clientArgs.User, clientArgs.Password);
        }

        public static void ProcesarJustificantesPersonal(ClientArgs clientArgs)
        {
            var records = CargarCsv<JpersonalInterchageModel, JpersonalInterchageModelCsvMap>(clientArgs.File, clientArgs.Encoding);
            _logger.Info("Cargado fichero {0}", clientArgs.File);
            SendRecords(records.Cast<JInterchageModel>().ToList(), clientArgs.User, clientArgs.Password);
        }

        public static void ProcesarJustificantesViajes(ClientArgs clientArgs)
        {
            var records = CargarCsv<JviajeInterchageModel, JviajesInterchageModelCsvMap>(clientArgs.File, clientArgs.Encoding);
            _logger.Info("Cargado fichero {0}", clientArgs.File);
            SendRecords(records.Cast<JInterchageModel>().ToList(), clientArgs.User, clientArgs.Password);
        }

        /// <summary>
        /// Procesa el fichero CSV de entrada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        private static List<T> CargarCsv<T, U>(string path, Codificacion codificacion) where T : JInterchageModel where U : CsvClassMap
        {
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

        /// <summary>
        /// Envio de datos al WS - JUSTIFICACIÓN
        /// </summary>
        /// <param name="records">Listado que contiene los valores de modelo de datos de Justificación del Servicio Web</param>
        /// <returns></returns>
        private static List<LoadResult> SendRecords(List<JInterchageModel> records, string usuario, string password)
        {
            try
            {
                #region Descomentar código para ignorar certificados

                //System.Net.ServicePointManager.ServerCertificateValidationCallback =
                //delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                //{ return true; };

                #endregion

                var client = new JustificationClient();
                client.ClientCredentials.UserName.UserName = usuario;
                client.ClientCredentials.UserName.Password = password;
                client.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;

                var resultadosEnvio = new List<LoadResult>();
                int total = records.Count;

                foreach (var item in records.Select((value, i) => new { i, value }))
                {
                    LoadResult r;
                    _logger.Info("Cargando justificante {0} de {1}, Expediente {2}.", item.i + 1, total, item.value.Expediente);

                    switch (item.value)
                    {
                        case JbsInterchageModel justificanteBienesServicios:
                            // Mapeo de documentos
                            if (!string.IsNullOrEmpty(justificanteBienesServicios.NombreFicheroBSFactura))
                            {
                                justificanteBienesServicios.FicheroBSFactura = File.ReadAllBytes(justificanteBienesServicios.NombreFicheroBSFactura); //obtenemos los byte[] del fichero a través de la ruta que venga en el csv
                                justificanteBienesServicios.NombreFicheroBSFactura = Path.GetFileName(justificanteBienesServicios.NombreFicheroBSFactura); //obtenemos el nombre del fichero a través de la ruta que venga en el csv
                            }
                            if (!string.IsNullOrEmpty(justificanteBienesServicios.NombreFicheroBSPago))
                            {
                                justificanteBienesServicios.FicheroBSPago = File.ReadAllBytes(justificanteBienesServicios.NombreFicheroBSPago); //obtenemos los byte[] del fichero a través de la ruta que venga en el csv
                                justificanteBienesServicios.NombreFicheroBSPago = Path.GetFileName(justificanteBienesServicios.NombreFicheroBSPago); //obtenemos el nombre del fichero a través de la ruta que venga en el csv
                            }
                            if (!string.IsNullOrEmpty(justificanteBienesServicios.NombreFicheroBSOtros))
                            {
                                justificanteBienesServicios.FicheroBSOtros = File.ReadAllBytes(justificanteBienesServicios.NombreFicheroBSOtros); //obtenemos los byte[] del fichero a través de la ruta que venga en el csv
                                justificanteBienesServicios.NombreFicheroBSOtros = Path.GetFileName(justificanteBienesServicios.NombreFicheroBSOtros); //obtenemos el nombre del fichero a través de la ruta que venga en el csv
                            }

                            r = client.LoadJbs(justificanteBienesServicios); //Llamada al método LoadJbs del servicio web justificación

                            resultadosEnvio.Add(r);
                            break;

                        case JpersonalInterchageModel justificantePersonal:
                            // Mapeo de documentos
                            if (!string.IsNullOrEmpty(justificantePersonal.NombreFicheroPerNomina))
                            {
                                justificantePersonal.FicheroPerNomina = File.ReadAllBytes(justificantePersonal.NombreFicheroPerNomina);
                                justificantePersonal.NombreFicheroPerNomina = Path.GetFileName(justificantePersonal.NombreFicheroPerNomina);
                            }
                            if (!string.IsNullOrEmpty(justificantePersonal.NombreFicheroPerPago))
                            {
                                justificantePersonal.FicheroPerPago = File.ReadAllBytes(justificantePersonal.NombreFicheroPerPago);
                                justificantePersonal.NombreFicheroPerPago = Path.GetFileName(justificantePersonal.NombreFicheroPerPago);
                            }
                            if (!string.IsNullOrEmpty(justificantePersonal.NombreFicheroPerOtros))
                            {
                                justificantePersonal.FicheroPerOtros = File.ReadAllBytes(justificantePersonal.NombreFicheroPerOtros);
                                justificantePersonal.NombreFicheroPerOtros = Path.GetFileName(justificantePersonal.NombreFicheroPerOtros);
                            }

                            r = client.LoadJpersonal(justificantePersonal); //Llamada al método LoadJpersonal del servicio web justificación

                            resultadosEnvio.Add(r);
                            break;

                        case JviajeInterchageModel justificanteViajes:
                            // Mapeo de documentos
                            if (!string.IsNullOrEmpty(justificanteViajes.NombreFicheroViajeFactura))
                            {
                                justificanteViajes.FicheroViajeFactura = File.ReadAllBytes(justificanteViajes.NombreFicheroViajeFactura);
                                justificanteViajes.NombreFicheroViajeFactura = Path.GetFileName(justificanteViajes.NombreFicheroViajeFactura);
                            }
                            if (!string.IsNullOrEmpty(justificanteViajes.NombreFicheroViajePago))
                            {
                                justificanteViajes.FicheroViajePago = File.ReadAllBytes(justificanteViajes.NombreFicheroViajePago);
                                justificanteViajes.NombreFicheroViajePago = Path.GetFileName(justificanteViajes.NombreFicheroViajePago);
                            }
                            if (!string.IsNullOrEmpty(justificanteViajes.NombreFicheroViajeOtros))
                            {
                                justificanteViajes.FicheroViajeOtros = File.ReadAllBytes(justificanteViajes.NombreFicheroViajeOtros);
                                justificanteViajes.NombreFicheroViajeOtros = Path.GetFileName(justificanteViajes.NombreFicheroViajeOtros);
                            }

                            r = client.LoadJviajes(justificanteViajes); //Llamada al método LoadJviajes del servicio web justificación

                            resultadosEnvio.Add(r);
                            break;

                        default:
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

        }
    }
}