using Core.Entidades.Enti_Clases;
using Core.Entidades.Response.Res_ItemLista;

namespace BarraApp.Mobile.Services.ListaCompra;

public interface IItemListaService
{
    // Ruta api/itemlista/obtener (Método POST)
    Task<ResObtenerItemsLista> ObtenerAsync(Guid guidLista);

    // Ruta api/itemlista/agregar (Método POST)
    Task<ResAgregarItemLista> AgregarAsync(Guid guidLista, Guid guidProducto, decimal cantidad);

    // Ruta api/itemlista/actualizar (Método POST). Usa la fecha del item cargado para detectar conflictos.
    Task<ResActualizarItemLista> ActualizarAsync(ItemLista item, decimal cantidad, bool completado);

    // Ruta api/itemlista/eliminar (Método POST)
    Task<ResEliminarItemLista> EliminarAsync(Guid guidItem);
}
