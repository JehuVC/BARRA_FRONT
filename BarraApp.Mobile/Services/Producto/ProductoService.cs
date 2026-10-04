using BarraApp.Mobile.Services.Api;
using Core.Entidades.Response.Res_Producto;
using DTO.Producto;

namespace BarraApp.Mobile.Services.Producto;

public class ProductoService(HttpClient http) : ApiServiceBase(http), IProductoService
{
    public Task<ResBuscarProductos> BuscarAsync(string textoBusqueda) =>
        PostAsync<DTOBuscarProductos, ResBuscarProductos>("api/producto/buscar",
            new DTOBuscarProductos { textoBusqueda = textoBusqueda });
}
