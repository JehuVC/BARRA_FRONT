using BarraApp.Mobile.Services.Api;
using Core.Entidades.Response;
using DTO.Comercio;

namespace BarraApp.Mobile.Services.Comercio;

public class ComercioService(HttpClient http) : ApiServiceBase(http), IComercioService
{
    public Task<ResObtenerComerciosCercanos> ObtenerCercanosAsync(decimal latitud, decimal longitud, decimal radioKm) =>
        PostAsync<DTOObtenerComerciosCercanos, ResObtenerComerciosCercanos>("api/comercio/obtenerCercanos",
            new DTOObtenerComerciosCercanos { latitud = latitud, longitud = longitud, radioKm = radioKm });

    public Task<ResCrearComercio> CrearAsync(DTOCrearComercio dto) =>
        PostAsync<DTOCrearComercio, ResCrearComercio>("api/comercio/crear", dto);

    public Task<ResActualizarComercio> ActualizarAsync(DTOActualizarComercio dto) =>
        PostAsync<DTOActualizarComercio, ResActualizarComercio>("api/comercio/actualizar", dto);
}
