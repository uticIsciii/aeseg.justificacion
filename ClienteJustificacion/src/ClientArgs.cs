using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArgs;

namespace ClienteJustificacion
{
    public class ClientArgs
    {        // This argument is required and if not specified the user will 
        // be prompted.
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
    }

    public enum JustificacionFileType
    {
        Bienes,
        Viajes,
        Personal
    }
}
