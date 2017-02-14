using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aeseg.ProxyJustificacion;
using CsvHelper.Configuration;

namespace ClienteJustificacion
{
    public class JpersonalInterchageModelCsvMap : CsvClassMap<JpersonalInterchageModel>
    {
        public JpersonalInterchageModelCsvMap()
        {
            //ID_EXPEDIENTE;ID_PARTIDA;ID_CONCEPTO;ID_TIPO_JUSTIFICANTE;IMPORTE_TOTAL;IMPORTE_IMPUTADO;FECHA_PAGO;ANUALIDAD;OBSERVACIONES;DOC;
            //FECHA_DESDE;FECHA_HASTA;ID_CATEGORIA_LABORAL;NIF_PERSONAL

            Map(m => m.Expediente).Name("ID_EXPEDIENTE");
            Map(m => m.IdPartida).Name("ID_PARTIDA");
            Map(m => m.idConcepto).Name("ID_CONCEPTO");
            Map(m => m.idTipoJustificante).Name("ID_TIPO_JUSTIFICANTE");
            Map(m => m.ImporteTotal).Name("IMPORTE_TOTAL");
            Map(m => m.ImporteImputado).Name("IMPORTE_IMPUTADO");
            Map(m => m.FechaPago).Name("FECHA_PAGO");
            Map(m => m.Anualidad).Name("ANUALIDAD");
            Map(m => m.Observaciones).Name("OBSERVACIONES");
            Map(m => m.NombreFichero).Name("DOC").TypeConverter<FileTypeConverter>();

            Map(m => m.FechaDesde).Name("FECHA_DESDE");
            Map(m => m.FechaHasta).Name("FECHA_HASTA");
            Map(m => m.IdCategoriaLaboral).Name("ID_CATEGORIA_LABORAL");
            Map(m => m.Nif).Name("NIF_PERSONAL");
        }
    }
}
