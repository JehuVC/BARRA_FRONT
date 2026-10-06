using BarraApp.Mobile.Services.Api;

namespace BarraApp.Mobile.Services.ListaCompra;

public class AgregarAListaFlujo(IListaCompraService listaService, IItemListaService itemService) : IAgregarAListaFlujo
{
    public async Task<ResultadoAgregarALista> EjecutarAsync(Guid guidProducto, string nombreProducto)
    {
        try
        {
            var res = await listaService.ObtenerListasAsync();
            if (!res.resultado)
            {
                return new ResultadoAgregarALista(null, MensajesError.Traducir(res.error, "No se pudieron cargar tus listas."));
            }

            var listas = res.Listas ?? new();
            if (listas.Count == 0)
            {
                bool crear = await Shell.Current.DisplayAlertAsync(
                    "Aún no tienes listas",
                    "Crea tu primera lista de compras para poder agregar productos.",
                    "Crear lista",
                    "Ahora no");

                if (crear)
                {
                    // Nombre de la ruta de ListasPage (registrada en AppShell).
                    await Shell.Current.GoToAsync("ListasPage");
                }

                return new ResultadoAgregarALista(null, null);
            }

            // Numeradas para que dos listas con el mismo nombre no se confundan.
            var opciones = listas
                .Select((l, i) => $"{i + 1}. {l.Nombre}{(l.GuidGuild.HasValue ? "  👥" : string.Empty)}")
                .ToArray();

            string? elegida = await Shell.Current.DisplayActionSheetAsync(
                $"Agregar «{nombreProducto}» a...", "Cancelar", null, opciones);

            int indice = Array.IndexOf(opciones, elegida);
            if (indice < 0)
            {
                return new ResultadoAgregarALista(null, null);
            }

            var lista = listas[indice];
            var agregado = await itemService.AgregarAsync(lista.GuidLista, guidProducto, 1);
            if (!agregado.resultado)
            {
                return new ResultadoAgregarALista(null, MensajesError.Traducir(agregado.error, "No se pudo agregar el producto a la lista."));
            }

            return new ResultadoAgregarALista($"«{nombreProducto}» se agregó a «{lista.Nombre}».", null);
        }
        catch (ApiException ex)
        {
            return new ResultadoAgregarALista(null, ex.Message);
        }
    }
}
