using BarraApp.Mobile.Services.Api;
using Core.Entidades.Response;
using Core.Entidades.Response.Res_ListaCompra;
using DTO.ListaCompra;

namespace BarraApp.Mobile.Services.ListaCompra;

public class ListaCompraService(HttpClient http) : ApiServiceBase(http), IListaCompraService
{
    public Task<ResObtenerListasUsuario> ObtenerListasAsync() =>
        PostAsync<object, ResObtenerListasUsuario>("api/listacompra/ObtenerListasUsuario", new { });

    // Guid.Empty: el backend genera el GUID de la lista (solo se manda uno propio en modo offline).
    public Task<ResCrearListaCompra> CrearAsync(string nombre) =>
        PostAsync<DTOCrearListaCompra, ResCrearListaCompra>("api/listacompra/crear",
            new DTOCrearListaCompra { GuidLista = Guid.Empty, Nombre = nombre });

    public Task<ResActualizarListaCompra> ActualizarAsync(Guid guidLista, string nombre, DateTime? fechaModificacionCliente) =>
        PostAsync<DTOActualizarListaCompra, ResActualizarListaCompra>("api/listacompra/actualizar",
            new DTOActualizarListaCompra { GuidLista = guidLista, Nombre = nombre, FechaModificacionCliente = fechaModificacionCliente });

    public Task<ResEliminarListaCompra> EliminarAsync(Guid guidLista) =>
        PostAsync<DTOEliminarListaCompra, ResEliminarListaCompra>("api/listacompra/eliminar",
            new DTOEliminarListaCompra { GuidLista = guidLista });
}
