using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aeseg.ProxyJustificacion;
using CsvHelper.Configuration;

namespace ClienteJustificacion
{
    public class JviajesInterchageModelCsvMap : CsvClassMap<JviajeInterchageModel>
    {
        public JviajesInterchageModelCsvMap()
        {
            //ID_EXPEDIENTE;ID_PARTIDA;ID_CONCEPTO;ID_TIPO_JUSTIFICANTE;IMPORTE_TOTAL;IMPORTE_IMPUTADO;FECHA_PAGO;ANUALIDAD;OBSERVACIONES;DOC;
            //NUMERO_FACTURA;NIF;NOMBRE;CONCEPTO;FECHA_JUST;BASE_IMP;TIPO_IMP;
            //CENTRO_DESTINO;FECHA_INICIO_VIAJE; FECHA_INICIO_CONGRESO;FECHA_FIN_VIAJE;FECHA_FIN_CONGRESO;ID_PAIS;ID_TIPO_GASTO_VIAJE;ID_TIPO_VIAJE;ITINERARIO;NIF_VIAJERO

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
            Map(m => m.NifProveedor).Name("NIF");
            Map(m => m.NombreProveedor).Name("NOMBRE");
            Map(m => m.ConceptoFactura).Name("CONCEPTO");
            Map(m => m.FechaFactura).Name("FECHA_JUST");
            Map(m => m.BaseImponible).Name("BASE_IMP");
            Map(m => m.TipoImpositivo).Name("TIPO_IMP");

            Map(m => m.CentroDestino).Name("CENTRO_DESTINO");
            Map(m => m.FechaInicioViaje).Name("FECHA_INICIO_VIAJE");
            Map(m => m.FechaInicioCongreso).Name("FECHA_INICIO_CONGRESO");
            Map(m => m.FechaFinCongreso).Name("FECHA_FIN_CONGRESO");
            Map(m => m.FechaFinViaje).Name("FECHA_FIN_VIAJE");
            Map(m => m.IdPais).Name("ID_PAIS");
            Map(m => m.IdTipoGastoViaje).Name("ID_TIPO_GASTO_VIAJE");
            Map(m => m.IdTipoViaje).Name("ID_TIPO_VIAJE");
            Map(m => m.Itinerario).Name("ITINERARIO");
            Map(m => m.Nif).Name("NIF_VIAJERO");
        }
    }
}
