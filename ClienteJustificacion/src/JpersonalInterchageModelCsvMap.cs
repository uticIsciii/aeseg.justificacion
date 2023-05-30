using CsvHelper.Configuration;
using ISCIII.AESEG.ClienteJustificacion.Proxy.ProxyJustificacion;

namespace ISCIII.AESEG.ClienteJustificacion.BLL
{
    public class JpersonalInterchageModelCsvMap : CsvClassMap<JpersonalInterchageModel>
    {
        public JpersonalInterchageModelCsvMap()
        {
            // Campos del csv "JPERSONAL_WS.csv"
            Map(m => m.Expediente).Name("ID_EXPEDIENTE");
            Map(m => m.IdPartida).Name("ID_PARTIDA");
            Map(m => m.idConcepto).Name("ID_CONCEPTO");
            Map(m => m.idTipoJustificante).Name("TIPO_JUSTIFICANTE");
            Map(m => m.ImporteTotal).Name("IMPORTE_TOTAL");
            Map(m => m.ImporteImputado).Name("IMPORTE_IMPUTADO");
            Map(m => m.FechaPago).Name("FECHA_PAGO");
            Map(m => m.Observaciones).Name("OBSERVACIONES");
            Map(m => m.NombreFicheroPerNomina).Name("DOC_NOMINA").TypeConverter<FileTypeConverter>();
            Map(m => m.NombreFicheroPerPago).Name("DOC_PAGO").TypeConverter<FileTypeConverter>();
            Map(m => m.NombreFicheroPerOtros).Name("DOC_OTROS").TypeConverter<FileTypeConverter>();
            Map(m => m.FechaDesde).Name("FECHA_DESDE");
            Map(m => m.FechaHasta).Name("FECHA_HASTA");
            Map(m => m.IdCategoriaLaboral).Name("CATEGORIA_LABORAL");
            Map(m => m.Nif).Name("INVESTIGADOR");
            Map(m => m.DiasBajaIT).Name("DIAS_BAJA_IT (SOLO RRHH)");
        }
    }
}