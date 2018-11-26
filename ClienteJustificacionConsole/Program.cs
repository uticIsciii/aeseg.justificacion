using System;
using ISCIII.AESEG.ClienteJustificacion.BLL;
using NLog;
using PowerArgs;

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
                var argumentos = Args.Parse<ClientArgs>(args);
                _logger.Info("Proceso del fichero: {0}; {1}; {2}; {3};", argumentos.User, "*****", argumentos.FileType, argumentos.File);

                switch (argumentos.FileType)
                {
                    case JustificacionFileType.Bienes:
                        Process.ProcesarJustificantesBienesServicios(argumentos);
                        break;

                    case JustificacionFileType.Viajes:
                        Process.ProcesarJustificantesViajes(argumentos);
                        break;

                    case JustificacionFileType.Personal:
                        Process.ProcesarJustificantesPersonal(argumentos);
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