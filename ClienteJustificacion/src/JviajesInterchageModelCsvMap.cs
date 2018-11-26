using CsvHelper.Configuration;
using ISCIII.AESEG.ClienteJustificacion.Proxy.ProxyJustificacion;

namespace ISCIII.AESEG.ClienteJustificacion.BLL
{
    public class JviajesInterchageModelCsvMap : CsvClassMap<JviajeInterchageModel>
    {
        public JviajesInterchageModelCsvMap()
        {
            // Campos del csv "JVIAJES_WS.csv"
            Map(m => m.Expediente).Name("ID_EXPEDIENTE");
            Map(m => m.IdPartida).Name("ID_PARTIDA");
            Map(m => m.idConcepto).Name("ID_CONCEPTO");
            Map(m => m.ImporteTotal).Name("IMPORTE_TOTAL");
            Map(m => m.ImporteImputado).Name("IMPORTE_IMPUTADO");
            Map(m => m.FechaPago).Name("FECHA_PAGO");
            Map(m => m.idTipoJustificante).Name("TIPO_JUSTIFICANTE");
            Map(m => m.Observaciones).Name("OBSERVACIONES");
            Map(m => m.NombreFicheroViajeFactura).Name("DOC_FACTURA").TypeConverter<FileTypeConverter>();
            Map(m => m.NombreFicheroViajePago).Name("DOC_PAGO").TypeConverter<FileTypeConverter>();
            Map(m => m.NombreFicheroViajeOtros).Name("DOC_OTROS").TypeConverter<FileTypeConverter>();
            Map(m => m.NumeroFactura).Name("NUMERO_FACTURA");
            Map(m => m.NifProveedor).Name("NIF_PROVEEDOR");
            Map(m => m.NombreProveedor).Name("NOMBRE_PROVEEDOR");
            Map(m => m.ConceptoFactura).Name("CONCEPTO_FACTURA");
            Map(m => m.FechaFactura).Name("FECHA_FACTURA");
            Map(m => m.BaseImponible).Name("BASE_IMP");
            Map(m => m.TipoImpositivo).Name("TIPO_IMP");
            Map(m => m.CentroDestino).Name("CENTRO_DESTINO");
            Map(m => m.FechaInicioViaje).Name("FECHA_INICIO_VIAJE");
            Map(m => m.FechaInicioCongreso).Name("FECHA_INICIO_CONGRESO");
            Map(m => m.FechaFinCongreso).Name("FECHA_FIN_CONGRESO");
            Map(m => m.FechaFinViaje).Name("FECHA_FIN_VIAJE");
            Map(m => m.IdPais).Name("ID_PAIS");
            Map(m => m.IdTipoGastoViaje).Name("ID_TIPO_GASTO_VIAJE");
            Map(m => m.IdTipoViaje).Name("TIPO_VIAJE");
            Map(m => m.Itinerario).Name("ITINERARIO");
            Map(m => m.Nif).Name("INVESTIGADOR");
        }
    }
}