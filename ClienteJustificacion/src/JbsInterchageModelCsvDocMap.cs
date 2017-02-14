using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aeseg.ProxyJustificacion;
using CsvHelper.Configuration;


namespace ClienteJustificacion
{
    public sealed class JbsInterchageModelCsvDocMap : CsvClassMap<JbsInterchageModel>
    {
        public JbsInterchageModelCsvDocMap()
        {
            //ID_EXPEDIENTE;ID_PARTIDA;ID_CONCEPTO;TIPO_JUSTIFICANTE;IMPORTE_TOTAL;IMPORTE_IMPUTADO;FECHA_PAGO;NUMERO_FACTURA;NIF;NOMBRE;CONCEPTO;FECHA_JUST;BASE IMP;TIPO IMP;DOC

            Map(m => m.Expediente).Name("ID_EXPEDIENTE");
            Map(m => m.IdPartida).Name("ID_PARTIDA");
            Map(m => m.idConcepto).Name("ID_CONCEPTO");
            Map(m => m.idTipoJustificante).Name("TIPO_JUSTIFICANTE");
            Map(m => m.ImporteTotal).Name("IMPORTE_TOTAL");
            Map(m => m.ImporteImputado).Name("IMPORTE_IMPUTADO");
            Map(m => m.FechaPago).Name("FECHA_PAGO");
            Map(m => m.Anualidad).Name("ANUALIDAD");
            Map(m => m.Observaciones).Name("OBSERVACIONES");
            Map(m => m.NombreFichero).Name("DOC").TypeConverter<FileTypeConverter>();

            Map(m => m.NumeroFactura).Name("NUMERO_FACTURA");
            Map(m => m.NIF).Name("NIF");
            Map(m => m.NombreProveedor).Name("NOMBRE");
            Map(m => m.DescripcionConcepto).Name("CONCEPTO");
            Map(m => m.FechaFactura).Name("FECHA_JUST");
            Map(m => m.BaseImponible).Name("BASE_IMP");
            Map(m => m.TipoImpositivo).Name("TIPO_IMP");
        }
    }
}
