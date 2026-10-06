using Core.Entidades.Response;
using Core.Entidades.Response.Res_Historial;

namespace BarraApp.Mobile.Services.Historial;

public interface IHistorialService
{
    // Ruta api/historial/obtenerPorUsuario (Método POST, el usuario sale del JWT)
    Task<ResObtenerHistorial> ObtenerHistorialAsync();

    // Ruta api/historial/registrar (Método POST). El backend exige un término o un producto.
    Task<ResRegistrarBusqueda> RegistrarBusquedaAsync(string? terminoBusqueda, Guid? guidProducto = null);
}
