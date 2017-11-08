using ISCIII.AESEG.ClienteJustificacion.BLL;
using NLog.Targets;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace ISCIII.AESEG.ClienteJustificacion.Desktop
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private static Logger _logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();

            // Inicialización de NLog
            Target.Register<CustomTarget>("CajaTexto");
        }

        private async void Enviar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CajaResultados.Document.Blocks.Clear();

                bool formularioValido = ValidarFormulario();

                if (formularioValido)
                {
                    var usuario = CajaUsuario.Text;
                    var password = CajaPassword.Password.ToString();
                    var tipo = CajaTipo.Text;
                    var ruta = CajaRuta.Text;

                    JustificacionFileType tipoJustificacion = JustificacionFileType.Bienes;
                    switch (tipo)
                    {
                        case "Bienes":
                            tipoJustificacion = JustificacionFileType.Bienes;
                            break;

                        case "Viajes":
                            tipoJustificacion = JustificacionFileType.Viajes;
                            break;

                        case "Personal":
                            tipoJustificacion = JustificacionFileType.Personal;
                            break;

                        default:
                            CajaResultados.Document.Blocks.Add(new Paragraph(new Run("Debe seleccionar un tipo de justificante")));
                            break;
                    }

                    ClientArgs argumentos = new ClientArgs()
                    {
                        File = ruta,
                        FileType = tipoJustificacion,
                        Password = password,
                        User = usuario
                    };

                    CajaResultados.Document.Blocks.Add(new Paragraph(new Run("Procesando fichero...")));

                    Enviar.IsEnabled = false;

                    // Se crea una tarea para ejecutar en un hilo distinto para evitar que se quede colgada la interfaz mientras se ejecuta
                    Task tarea = Task.Run(() =>
                    {
                        switch (argumentos.FileType)
                        {
                            case JustificacionFileType.Bienes:
                                Process.ProcessJbs(argumentos);
                                break;

                            case JustificacionFileType.Viajes:
                                Process.ProcessJviajes(argumentos);
                                break;

                            case JustificacionFileType.Personal:
                                Process.ProcessJpersonal(argumentos);
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                    });

                    await tarea;
                } 
            }
            catch (System.IO.FileNotFoundException ex)
            {
                CajaResultados.Document.Blocks.Add(new Paragraph(new Run(ex.Message)));
            }
            catch (Exception ex)
            {
                CajaResultados.Document.Blocks.Add(new Paragraph(new Run(ex.Message)));
            }
            finally
            {
                Enviar.IsEnabled = true;
            }
        }

        private bool ValidarFormulario()
        {
            bool valido = true;

            if (string.IsNullOrEmpty(CajaUsuario.Text))
            {
                CajaResultados.Document.Blocks.Add(new Paragraph(new Run("El nombre de usuario es obligatorio")));
                valido = false;
            }

            if (string.IsNullOrEmpty(CajaPassword.Password.ToString()))
            {
                CajaResultados.Document.Blocks.Add(new Paragraph(new Run("El password es obligatorio")));
                valido = false;
            }

            var tipo = CajaTipo.Text;

            if (string.IsNullOrEmpty(tipo))
            {
                CajaResultados.Document.Blocks.Add(new Paragraph(new Run("Debe seleccionar un tipo de justificante")));
                valido = false;
            }

            switch (tipo)
            {
                case "Bienes":
                case "Viajes":
                case "Personal":
                    break;
                default:
                    CajaResultados.Document.Blocks.Add(new Paragraph(new Run("El tipo de justificante no es válido")));
                    valido = false;
                    break;
            }

            if (string.IsNullOrEmpty(CajaRuta.Text))
            {
                CajaResultados.Document.Blocks.Add(new Paragraph(new Run("Debe seleccionar una ruta")));
                valido = false;
            }

            return valido;
        }

        private void Examinar_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            var result = fileDialog.ShowDialog();
            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    var file = fileDialog.FileName;
                    CajaRuta.Text = file;
                    break;

                case System.Windows.Forms.DialogResult.Cancel:
                default:
                    break;
            }
        }
    }
}