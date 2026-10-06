using System.Globalization;
using BarraApp.Mobile.Helpers;

namespace BarraApp.Mobile.ViewModels.Historial;

// Una busqueda (o producto visto) del historial, ya sin repetidos. "Texto" es lo que se muestra y lo que se
// vuelve a buscar al tocarla.
public class HistorialItemViewModel(string texto, bool esProducto, DateTime fechaUtc, int veces, DateTime ahoraUtc)
{
    public string Texto { get; } = texto;
    public bool EsProducto { get; } = esProducto;
    public DateTime FechaUtc { get; } = fechaUtc;
    public int Veces { get; } = veces;

    public string Icono => EsProducto ? "🛍️" : "🔍";
    public string Tipo => EsProducto ? "Producto visto" : "Búsqueda";
    public bool TieneVeces => Veces > 1;
    public string VecesTexto => $"{Veces} veces";

    // Hoy se muestra la hora (en la zona del telefono); antes, "ayer" / "hace N dias" / la fecha.
    public string Cuando => FechaUtc.Date >= ahoraUtc.Date
        ? DateTime.SpecifyKind(FechaUtc, DateTimeKind.Utc).ToLocalTime().ToString("HH:mm", CultureInfo.InvariantCulture)
        : Formato.Hace(FechaUtc, ahoraUtc);
}

// Un bloque del historial ("Hoy", "Ayer"...) para el CollectionView agrupado.
public class GrupoHistorial(string titulo, IEnumerable<HistorialItemViewModel> items) : List<HistorialItemViewModel>(items)
{
    public string Titulo { get; } = titulo;
}
