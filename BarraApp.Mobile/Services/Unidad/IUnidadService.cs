using Core.Entidades.Response.Res_Unidad;

namespace BarraApp.Mobile.Services.Unidad;

public interface IUnidadService
{
    // Ruta api/unidad/obtener (Método POST)
    Task<ResObtenerUnidades> ObtenerAsync();

    // Ruta api/unidad/crear (Método POST). La abreviatura es opcional (max 10 caracteres).
    Task<ResCrearUnidad> CrearAsync(string nombre, string? abreviatura = null);
}
