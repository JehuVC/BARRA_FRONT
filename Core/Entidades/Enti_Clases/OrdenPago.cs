using System;

namespace Core.Entidades.Enti_Clases
{
    public class OrdenPago
    {
        public Guid guidOrdenPago { get; set; }
        public decimal monto { get; set; }
        public string moneda { get; set; }
        public string estado { get; set; }
        public string urlCheckout { get; set; }
        public string codigoRespuesta { get; set; }
        public string codigoDescripcion { get; set; }
        public DateTime fechaCreacion { get; set; }
        public DateTime fechaActualizacion { get; set; }
    }
}
