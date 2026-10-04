using Core.Entidades.Response;
using Core.Entidades.Response.Res_ListaCompra;

namespace BarraApp.Mobile.Services.ListaCompra;

public interface IListaCompraService
{
    // Ruta api/listacompra/ObtenerListasUsuario (Método POST, el usuario sale del JWT)
    Task<ResObtenerListasUsuario> ObtenerListasAsync();

    // Ruta api/listacompra/crear (Método POST). El guild lo resuelve el backend desde el usuario.
    Task<ResCrearListaCompra> CrearAsync(string nombre);

    // Ruta api/listacompra/actualizar (Método POST). fechaModificacionCliente activa la deteccion de conflictos.
    Task<ResActualizarListaCompra> ActualizarAsync(Guid guidLista, string nombre, DateTime? fechaModificacionCliente);

    // Ruta api/listacompra/eliminar (Método POST)
    Task<ResEliminarListaCompra> EliminarAsync(Guid guidLista);
}
