using Core.Entidades.Response;

namespace BarraApp.Mobile.Services.Categoria;

public interface ICategoriaService
{
    // Ruta api/categoria/obtener (Método POST)
    Task<ResObtenerCategorias> ObtenerAsync();

    // Ruta api/categoria/crear (Método POST). La descripcion es opcional.
    Task<ResCrearCategoria> CrearAsync(string nombre, string? descripcion = null);
}
