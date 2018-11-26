using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using NLog;
using NLog.Targets;

namespace ISCIII.AESEG.ClienteJustificacion.Desktop
{
    [Target("CajaTexto")]
    public sealed class CustomTarget : TargetWithLayout
    {
        public CustomTarget()
        {
        }

        protected override void Write(LogEventInfo logEvent)
        {
            string mensaje = Layout.Render(logEvent);

            // Se hace una llamada para que en background actualice en el thread de interfaz la caja de resultados con el nuevo mensaje
            Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    (Action)delegate ()
                    {
                        ActualizarCajaResultados(mensaje);
                    }
            );
        }

        private void ActualizarCajaResultados(string mensaje)
        {
            var ventanaPrincipal = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;

            ventanaPrincipal.CajaResultados.Document.Blocks.Add(new Paragraph(new Run(mensaje)));
        }
    }
}