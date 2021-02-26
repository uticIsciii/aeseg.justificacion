using PowerArgs;

namespace ISCIII.AESEG.ClienteJustificacion.BLL
{
    public class ClientArgs
    {
        [ArgRequired()]
        [ArgShortcut("u")]
        [ArgDescription("Nombre de usuario.")]
        public string User { get; set; }

        [ArgRequired()]
        [ArgShortcut("p")]
        [ArgDescription("Contraseña.")]
        public string Password { get; set; }

        [ArgRequired()]
        [ArgShortcut("t")]
        [ArgDescription("Tipo de justificantes.")]
        public JustificacionFileType FileType { get; set; }

        [ArgRequired()]
        [ArgShortcut("f")]
        [ArgExistingFile]
        [ArgDescription("Ruta del fichero csv.")]
        public string File { get; set; }

        [ArgShortcut("c")]
        [ArgExistingFile]
        [ArgDescription("Codificación de fichero.")]
        public Codificacion Encoding { get; set; }
    }

    public enum JustificacionFileType
    {
        Bienes,
        Viajes,
        Personal
    }

    public enum Codificacion
    {
        UTF8,
        Windows1252
    }
}