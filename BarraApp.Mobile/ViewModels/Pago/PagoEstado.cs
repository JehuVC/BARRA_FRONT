using BarraApp.Mobile.Helpers;
using Core.Entidades.Enti_Clases;

namespace BarraApp.Mobile.ViewModels.Pago;

// Lo que se le muestra al usuario segun el estado de una orden de pago (PENDIENTE / PAGADO / RECHAZADO).
public record ResultadoPago(string Icono, string Titulo, string Mensaje, string Detalle, bool Exitoso, bool Pendiente);

public static class PagoEstado
{
    public const string Pendiente = "PENDIENTE";
    public const string Pagado = "PAGADO";
    public const string Rechazado = "RECHAZADO";

    public static bool EsFinal(string? estado) =>
        string.Equals(estado, Pagado, StringComparison.OrdinalIgnoreCase)
        || string.Equals(estado, Rechazado, StringComparison.OrdinalIgnoreCase);

    public static ResultadoPago Resolver(OrdenPago orden)
    {
        string monto = Formato.Monto(orden.monto, orden.moneda);
        string fecha = DateTime.SpecifyKind(orden.fechaActualizacion, DateTimeKind.Utc).ToLocalTime().ToString("dd/MM/yyyy h:mm tt");
        string detalle = $"{monto} · {fecha}";

        if (string.Equals(orden.estado, Pagado, StringComparison.OrdinalIgnoreCase))
        {
            return new ResultadoPago("✅", "¡Pago aprobado!", $"Recibimos tu pago de {monto}. ¡Gracias por apoyar BarraApp!", detalle, true, false);
        }

        if (string.Equals(orden.estado, Rechazado, StringComparison.OrdinalIgnoreCase))
        {
            string motivo = string.IsNullOrWhiteSpace(orden.codigoDescripcion)
                ? "El banco no aprobó el pago."
                : $"Motivo: {orden.codigoDescripcion.Trim()}.";
            return new ResultadoPago("❌", "Pago rechazado", $"{motivo} Puedes intentarlo de nuevo con otra tarjeta.", detalle, false, false);
        }

        return new ResultadoPago("⏳", "Pago en proceso", "Todavía no recibimos la confirmación del pago. Si ya pagaste, puede tardar unos segundos.", detalle, false, true);
    }
}
