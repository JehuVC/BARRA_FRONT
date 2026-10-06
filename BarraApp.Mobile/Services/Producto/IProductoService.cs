using Core.Entidades.Response;
using Core.Entidades.Response.Res_Producto;
using DTO.Producto;

namespace BarraApp.Mobile.Services.Producto;

public interface IProductoService
{
    // Ruta api/producto/buscar (Método POST). Busca por nombre o marca; el backend exige texto o categoría.
    Task<ResBuscarProductos> BuscarAsync(string textoBusqueda, Guid? guidCategoria = null);

    // Ruta api/producto/obtenerPorCodigo (Método POST)
    Task<ResObtenerProducto> ObtenerPorCodigoAsync(string codigoBarras);

    // Ruta api/producto/registrarPrecio (Método POST). Si el comercio ya tenia precio para el producto, lo actualiza.
    Task<ResRegistrarPrecio> RegistrarPrecioAsync(Guid guidProducto, Guid guidComercio, decimal precio);

    // Ruta api/producto/crear (Método POST). El backend rechaza un codigo de barras que ya existe.
    Task<ResCrearProducto> CrearAsync(DTOCrearProducto dto);

    // Ruta api/producto/actualizar (Método POST). No cambia el codigo de barras.
    Task<ResActualizarProducto> ActualizarAsync(DTOActualizarProducto dto);
}
