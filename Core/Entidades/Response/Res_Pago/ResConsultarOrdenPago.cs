using Core.Entidades.Enti_Clases;

namespace Core.Entidades.Response.Res_Pago
{
    public class ResConsultarOrdenPago : ResBase
    {
        public OrdenPago Orden { get; set; }
    }
}
