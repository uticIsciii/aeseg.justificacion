using ISCIII.AESEG.ClienteJustificacion.BLL;
using NLog;
using PowerArgs;
using System;

namespace ISCIII.AESEG.ClienteJustificacion.Console
{
    public class Program
    {
        #region PROPIEDADES

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion PROPIEDADES

        #region EJECUCIÓN DEL PROCESO INICIAL

        public static void Main(string[] args)
        {
            try
            {
                var clientArgs = Args.Parse<ClientArgs>(args);
                _logger.Info("Proceso del fichero: {0}; {1}; {2}; {3};", clientArgs.User, "*****", clientArgs.FileType, clientArgs.File);

                switch (clientArgs.FileType)
                {
                    case JustificacionFileType.Bienes:
                        Process.ProcessJbs(clientArgs);
                        break;

                    case JustificacionFileType.Viajes:
                        Process.ProcessJviajes(clientArgs);
                        break;

                    case JustificacionFileType.Personal:
                        Process.ProcessJpersonal(clientArgs);
                        break;

                    default:
                        _logger.Warn("El tipo de justificante seleccionado no es válido");
                        break;
                }
            }
            catch (ArgException ex)
            {
                _logger.Warn(ex.Message);
                _logger.Warn(ArgUsage.GenerateUsageFromTemplate<ClientArgs>());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        #endregion EJECUCIÓN DEL PROCESO INICIAL
    }
}