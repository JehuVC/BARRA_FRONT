using BarraApp.Mobile.Services.Api;
using Core.Entidades.Response;
using DTO.Categoria;

namespace BarraApp.Mobile.Services.Categoria;

public class CategoriaService(HttpClient http) : ApiServiceBase(http), ICategoriaService
{
    public Task<ResObtenerCategorias> ObtenerAsync() =>
        PostAsync<object, ResObtenerCategorias>("api/categoria/obtener", new { });

    public Task<ResCrearCategoria> CrearAsync(string nombre, string? descripcion = null) =>
        PostAsync<DTOCrearCategoria, ResCrearCategoria>("api/categoria/crear",
            new DTOCrearCategoria { nombre = nombre, descripcion = descripcion! });
}
