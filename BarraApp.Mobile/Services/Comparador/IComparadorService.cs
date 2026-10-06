using Core.Entidades.Response;

namespace BarraApp.Mobile.Services.Comparador;

public interface IComparadorService
{
    // Ruta api/comparador/comparar (Método POST). Devuelve los precios ordenados de menor a mayor.
    // Con latitud y longitud, cada precio trae tambien la distancia al comercio.
    Task<ResCompararPrecios> CompararAsync(string codigoBarras, decimal? latitud = null, decimal? longitud = null);
}
