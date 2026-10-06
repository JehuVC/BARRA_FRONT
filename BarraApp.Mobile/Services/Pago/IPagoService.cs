using Core.Entidades.Response.Res_Pago;

namespace BarraApp.Mobile.Services.Pago;

public interface IPagoService
{
    // Ruta api/pago/crear (Método POST). Crea la orden PENDIENTE y devuelve la URL de checkout de Tilopay.
    // Moneda: "CRC" o "USD". Monto: mayor que 0, maximo 2 decimales.
    Task<ResCrearOrdenPago> CrearAsync(decimal monto, string moneda);

    // Ruta api/pago/consultar (Método POST). Solo devuelve ordenes del usuario logueado. Si la orden sigue
    // PENDIENTE, el backend le pregunta a Tilopay antes de responder.
    Task<ResConsultarOrdenPago> ConsultarAsync(Guid guidOrdenPago);
}
