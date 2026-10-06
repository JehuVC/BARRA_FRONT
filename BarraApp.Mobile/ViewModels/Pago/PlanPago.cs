using BarraApp.Mobile.Helpers;

namespace BarraApp.Mobile.ViewModels.Pago;

// Un plan que el usuario puede pagar. El backend no tiene catalogo de planes (api/pago/crear recibe un monto y una
// moneda), asi que los planes viven aqui. Moneda: "CRC" o "USD".
public record PlanPago(string Id, string Nombre, string Descripcion, decimal Monto, string Moneda, bool Destacado = false)
{
    public string PrecioTexto => Formato.Monto(Monto, Moneda);
}

public static class PlanesPago
{
    // El backend usa Tilopay en modo "sandbox" (no se cobra dinero real). Poner en false cuando se pase a produccion.
    public const bool ModoPruebas = true;

    // TODO(equipo): precios y beneficios de ejemplo, confirmar antes de la presentacion.
    public static readonly IReadOnlyList<PlanPago> Todos = new[]
    {
        new PlanPago("mensual", "Premium mensual", "Sin anuncios, alertas de precios y gremios más grandes.", 2500m, "CRC"),
        new PlanPago("anual", "Premium anual", "Lo mismo que el mensual con 2 meses gratis.", 25000m, "CRC", Destacado: true)
    };
}
