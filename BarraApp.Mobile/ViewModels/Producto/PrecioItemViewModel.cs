using BarraApp.Mobile.Helpers;
using Core.Entidades.Enti_Clases;

namespace BarraApp.Mobile.ViewModels.Producto;

// Una fila de la comparacion de precios. El backend ya manda la lista ordenada de menor a mayor precio
// y calcula cuanto se ahorra frente al comercio mas caro; aqui solo se arman los textos para mostrar.
public class PrecioItemViewModel(PrecioComparacion precio, bool esMejor)
{
    public PrecioComparacion Precio { get; } = precio;

    public bool EsMejor { get; } = esMejor;
    public Guid GuidComercio => Precio.guidComercio;
    public string Comercio => Precio.nombreComercio ?? string.Empty;

    // "Walmart · Calle 5, San José"
    public string Detalle
    {
        get
        {
            var partes = new[] { Precio.cadena, Precio.direccion }.Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(" · ", partes);
        }
    }

    public bool TieneDetalle => !string.IsNullOrWhiteSpace(Detalle);
    public string PrecioTexto => Formato.Colones(Precio.precio);
    public string Actualizado => $"Actualizado {Formato.Hace(Precio.fechaActualizacion)}";

    public bool TieneDistancia => Precio.distanciaKm.HasValue;
    public string Distancia => Precio.distanciaKm.HasValue ? Formato.Distancia(Precio.distanciaKm.Value) : string.Empty;

    public bool TieneAhorro => Precio.montoDescuento > 0;
    public string Ahorro => TieneAhorro
        ? $"Ahorras {Formato.Colones(Precio.montoDescuento)} ({Precio.porcentajeDescuento.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture)}%) vs. el más caro"
        : string.Empty;
}
