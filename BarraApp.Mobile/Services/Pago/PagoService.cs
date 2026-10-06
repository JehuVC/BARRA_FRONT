using BarraApp.Mobile.Services.Api;
using Core.Entidades.Response.Res_Pago;
using DTO.Pago;

namespace BarraApp.Mobile.Services.Pago;

public class PagoService(HttpClient http) : ApiServiceBase(http), IPagoService
{
    public Task<ResCrearOrdenPago> CrearAsync(decimal monto, string moneda) =>
        PostAsync<DTOCrearOrdenPago, ResCrearOrdenPago>("api/pago/crear",
            new DTOCrearOrdenPago { Monto = monto, Moneda = moneda });

    public Task<ResConsultarOrdenPago> ConsultarAsync(Guid guidOrdenPago) =>
        PostAsync<DTOConsultarOrdenPago, ResConsultarOrdenPago>("api/pago/consultar",
            new DTOConsultarOrdenPago { GuidOrdenPago = guidOrdenPago });
}
