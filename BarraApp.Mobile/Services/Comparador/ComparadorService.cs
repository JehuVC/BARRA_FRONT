using BarraApp.Mobile.Services.Api;
using Core.Entidades.Response;
using DTO.Comparador;

namespace BarraApp.Mobile.Services.Comparador;

public class ComparadorService(HttpClient http) : ApiServiceBase(http), IComparadorService
{
    public Task<ResCompararPrecios> CompararAsync(string codigoBarras, decimal? latitud = null, decimal? longitud = null) =>
        PostAsync<DTOCompararPrecios, ResCompararPrecios>("api/comparador/comparar",
            new DTOCompararPrecios { codigoBarras = codigoBarras, latitud = latitud, longitud = longitud });
}
