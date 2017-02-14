using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.TypeConversion;

namespace ClienteJustificacion
{
    /// <summary>
    /// Conversor para rutas de documentos. Valida la existencia del fichero.
    /// </summary>
    public class FileTypeConverter : StringConverter
    {
        public override object ConvertFromString(TypeConverterOptions options, string text)
        {
            string r = base.ConvertFromString(options, text).ToString();
            if (!string.IsNullOrWhiteSpace(r) && !File.Exists(r)) throw new ArgumentException("No se encuentra el fichero.");
            return r;
        }
    }
}
