using BarraApp.Mobile.Services.Api;
using Core.Entidades.Response.Res_Unidad;
using DTO.Unidad;

namespace BarraApp.Mobile.Services.Unidad;

public class UnidadService(HttpClient http) : ApiServiceBase(http), IUnidadService
{
    public Task<ResObtenerUnidades> ObtenerAsync() =>
        PostAsync<object, ResObtenerUnidades>("api/unidad/obtener", new { });

    public Task<ResCrearUnidad> CrearAsync(string nombre, string? abreviatura = null) =>
        PostAsync<DTOCrearUnidad, ResCrearUnidad>("api/unidad/crear",
            new DTOCrearUnidad { nombre = nombre, abreviatura = abreviatura! });
}
