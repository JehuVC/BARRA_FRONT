using System;

namespace Core.Entidades.Request.Req_Pago
{
    public class ReqCrearOrdenPago
    {
        public Guid GuidUsuario { get; set; }
        public decimal Monto { get; set; }
        public string Moneda { get; set; }
    }
}
