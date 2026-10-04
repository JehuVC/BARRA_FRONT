using System;

namespace Core.Entidades.Request.Req_Pago
{
    public class ReqConsultarOrdenPago
    {
        public Guid GuidOrdenPago { get; set; }
        public Guid GuidUsuario { get; set; }
    }
}
