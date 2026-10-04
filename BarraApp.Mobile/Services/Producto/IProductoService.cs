using Core.Entidades.Response.Res_Producto;

namespace BarraApp.Mobile.Services.Producto;

public interface IProductoService
{
    // Ruta api/producto/buscar (Método POST). Busca por nombre o marca.
    Task<ResBuscarProductos> BuscarAsync(string textoBusqueda);
}
