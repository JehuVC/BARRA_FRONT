using BarraApp.Mobile.Services.Api;
using Core.Entidades.Response;
using Core.Entidades.Response.Res_Producto;
using DTO.Producto;

namespace BarraApp.Mobile.Services.Producto;

public class ProductoService(HttpClient http) : ApiServiceBase(http), IProductoService
{
    public Task<ResBuscarProductos> BuscarAsync(string textoBusqueda, Guid? guidCategoria = null) =>
        PostAsync<DTOBuscarProductos, ResBuscarProductos>("api/producto/buscar",
            new DTOBuscarProductos { textoBusqueda = textoBusqueda, guidCategoria = guidCategoria });

    public Task<ResObtenerProducto> ObtenerPorCodigoAsync(string codigoBarras) =>
        PostAsync<DTOObtenerProducto, ResObtenerProducto>("api/producto/obtenerPorCodigo",
            new DTOObtenerProducto { codigoBarras = codigoBarras });

    public Task<ResRegistrarPrecio> RegistrarPrecioAsync(Guid guidProducto, Guid guidComercio, decimal precio) =>
        PostAsync<DTORegistrarPrecio, ResRegistrarPrecio>("api/producto/registrarPrecio",
            new DTORegistrarPrecio { GuidProducto = guidProducto, GuidComercio = guidComercio, Precio = precio });

    public Task<ResCrearProducto> CrearAsync(DTOCrearProducto dto) =>
        PostAsync<DTOCrearProducto, ResCrearProducto>("api/producto/crear", dto);

    public Task<ResActualizarProducto> ActualizarAsync(DTOActualizarProducto dto) =>
        PostAsync<DTOActualizarProducto, ResActualizarProducto>("api/producto/actualizar", dto);
}
