using BarraApp.Mobile.Services.Api;
using Core.Entidades.Enti_Clases;
using Core.Entidades.Response.Res_ItemLista;
using DTO.ItemLista;

namespace BarraApp.Mobile.Services.ListaCompra;

public class ItemListaService(HttpClient http) : ApiServiceBase(http), IItemListaService
{
    public Task<ResObtenerItemsLista> ObtenerAsync(Guid guidLista) =>
        PostAsync<DTOObtenerItemsLista, ResObtenerItemsLista>("api/itemlista/obtener",
            new DTOObtenerItemsLista { GuidLista = guidLista });

    public Task<ResAgregarItemLista> AgregarAsync(Guid guidLista, Guid guidProducto, decimal cantidad) =>
        PostAsync<DTOAgregarItemLista, ResAgregarItemLista>("api/itemlista/agregar",
            new DTOAgregarItemLista { GuidLista = guidLista, GuidProducto = guidProducto, Cantidad = cantidad });

    public Task<ResActualizarItemLista> ActualizarAsync(ItemLista item, decimal cantidad, bool completado) =>
        PostAsync<DTOActualizarItemLista, ResActualizarItemLista>("api/itemlista/actualizar",
            new DTOActualizarItemLista
            {
                GuidItem = item.guidItem,
                Cantidad = cantidad,
                PrecioEstimado = item.precioEstimado,
                Completado = completado,
                FechaModificacionCliente = item.fechaModificacion
            });

    public Task<ResEliminarItemLista> EliminarAsync(Guid guidItem) =>
        PostAsync<DTOEliminarItemLista, ResEliminarItemLista>("api/itemlista/eliminar",
            new DTOEliminarItemLista { GuidItem = guidItem });
}
