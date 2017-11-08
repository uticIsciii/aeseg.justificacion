using CsvHelper.Configuration;
using ISCIII.AESEG.ClienteJustificacion.Proxy.ProxyJustificacion;

namespace ISCIII.AESEG.ClienteJustificacion.BLL
{
    public sealed class JbsInterchageModelCsvDocMap : CsvClassMap<JbsInterchageModel>
    {
        public JbsInterchageModelCsvDocMap()
        {
            //Campos del csv "BIENES_SERV_.csv"
            //ID_EXPEDIENTE;ID_PARTIDA;ID_CONCEPTO;IMPORTE_TOTAL;IMPORTE_IMPUTADO;FECHA_PAGO;TIPO_JUSTIFICANTE;ANUALIDAD;OBSERVACIONES;NUMERO_FACTURA;NIF;NOMBRE;CONCEPTO;FECHA_JUST;BASE_IMP;TIPO_IMP;DOC_FACTURA;DOC_PAGO;DOC_OTROS

            Map(m => m.Expediente).Name("ID_EXPEDIENTE");
            Map(m => m.IdPartida).Name("ID_PARTIDA"); //Partida a la que se imputa el gasto
            Map(m => m.idConcepto).Name("ID_CONCEPTO"); //Concepto al que se imputa el gasto
            Map(m => m.ImporteTotal).Name("IMPORTE_TOTAL");
            Map(m => m.ImporteImputado).Name("IMPORTE_IMPUTADO"); //Parte del importe total que se imputa a la subvención
            Map(m => m.FechaPago).Name("FECHA_PAGO");
            Map(m => m.idTipoJustificante).Name("TIPO_JUSTIFICANTE"); //Puede ser de tres tipos: Factura, ticket o albarán
            Map(m => m.Anualidad).Name("ANUALIDAD"); //Año al que se imputa el gasto
            Map(m => m.Observaciones).Name("OBSERVACIONES");
            Map(m => m.NumeroFactura).Name("NUMERO_FACTURA");
            Map(m => m.NIF).Name("NIF"); //Nif o Cif del Proveedor
            Map(m => m.NombreProveedor).Name("NOMBRE"); //Nombre del Proveedor
            Map(m => m.DescripcionConcepto).Name("CONCEPTO");
            Map(m => m.FechaFactura).Name("FECHA_JUST");
            Map(m => m.BaseImponible).Name("BASE_IMP");
            Map(m => m.TipoImpositivo).Name("TIPO_IMP");
            //Documentos justificativos
            Map(m => m.NombreFicheroBSFactura).Name("DOC_FACTURA").TypeConverter<FileTypeConverter>();
            Map(m => m.NombreFicheroBSPago).Name("DOC_PAGO").TypeConverter<FileTypeConverter>();
            Map(m => m.NombreFicheroBSOtros).Name("DOC_OTROS").TypeConverter<FileTypeConverter>();
        }
    }
}