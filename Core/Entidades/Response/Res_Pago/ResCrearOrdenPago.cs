using System;

namespace Core.Entidades.Response.Res_Pago
{
    public class ResCrearOrdenPago : ResBase
    {
        public Guid GuidOrdenPago { get; set; }
        public string UrlCheckout { get; set; }
    }
}
