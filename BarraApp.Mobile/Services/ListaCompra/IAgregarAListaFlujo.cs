namespace BarraApp.Mobile.Services.ListaCompra;

// Resultado de "agregar un producto a una lista". Exito y Error son null los dos si el usuario canceló.
public record ResultadoAgregarALista(string? Exito, string? Error);

// Flujo compartido por las pantallas que agregan un producto a una lista de compra (búsqueda, detalle...):
// carga las listas del usuario, le deja elegir una y agrega el producto con cantidad 1.
public interface IAgregarAListaFlujo
{
    Task<ResultadoAgregarALista> EjecutarAsync(Guid guidProducto, string nombreProducto);
}
