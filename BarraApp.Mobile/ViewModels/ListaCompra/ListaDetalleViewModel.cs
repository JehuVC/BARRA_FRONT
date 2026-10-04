using System.Collections.ObjectModel;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.ListaCompra;
using BarraApp.Mobile.Services.Producto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entidades.Enti_Clases;
using ProductoEntidad = Core.Entidades.Enti_Clases.Producto;

namespace BarraApp.Mobile.ViewModels.ListaCompra;

// FE-S10-01: ver, agregar, editar (cantidad / completado) y eliminar productos de una lista.
// Para agregar se busca el producto por nombre o marca (api/producto/buscar).
public partial class ListaDetalleViewModel(IItemListaService itemService, IProductoService productoService)
    : ObservableObject, IQueryAttributable
{
    private Guid _guidLista;

    [ObservableProperty] private string nombreLista = string.Empty;
    [ObservableProperty] private bool estaOcupado;
    [ObservableProperty] private bool cargaInicialTerminada;
    [ObservableProperty] private string mensajeError = string.Empty;

    // Buscador de productos
    [ObservableProperty] private bool buscadorVisible;
    [ObservableProperty] private string textoBusqueda = string.Empty;
    [ObservableProperty] private bool buscando;
    [ObservableProperty] private bool busquedaRealizada;

    public ObservableCollection<ItemLista> Items { get; } = new();
    public ObservableCollection<ProductoEntidad> Resultados { get; } = new();

    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool PuedeInteractuar => !EstaOcupado;
    public bool ListaVacia => CargaInicialTerminada && Items.Count == 0;
    public bool SinResultados => BusquedaRealizada && Resultados.Count == 0;
    public string TextoBotonBuscador => BuscadorVisible ? "Cerrar" : "+ Agregar producto";

    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));
    partial void OnEstaOcupadoChanged(bool value) => OnPropertyChanged(nameof(PuedeInteractuar));
    partial void OnCargaInicialTerminadaChanged(bool value) => OnPropertyChanged(nameof(ListaVacia));
    partial void OnBusquedaRealizadaChanged(bool value) => OnPropertyChanged(nameof(SinResultados));
    partial void OnBuscadorVisibleChanged(bool value) => OnPropertyChanged(nameof(TextoBotonBuscador));

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("guidLista", out var guid) && Guid.TryParse(guid?.ToString(), out var parsed))
        {
            _guidLista = parsed;
        }

        if (query.TryGetValue("nombre", out var nombre))
        {
            NombreLista = Uri.UnescapeDataString(nombre?.ToString() ?? string.Empty);
        }
    }

    [RelayCommand]
    public async Task CargarAsync()
    {
        if (EstaOcupado || _guidLista == Guid.Empty) return;
        EstaOcupado = true;
        MensajeError = string.Empty;

        try
        {
            await RecargarItemsAsync();
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            CargaInicialTerminada = true;
            EstaOcupado = false;
        }
    }

    [RelayCommand]
    private void AlternarBuscador()
    {
        BuscadorVisible = !BuscadorVisible;
        if (!BuscadorVisible)
        {
            TextoBusqueda = string.Empty;
            Resultados.Clear();
            BusquedaRealizada = false;
        }
    }

    [RelayCommand]
    private async Task BuscarAsync()
    {
        if (Buscando) return;
        MensajeError = string.Empty;

        string texto = TextoBusqueda.Trim();
        if (texto.Length == 0)
        {
            MensajeError = "Escribe qué producto buscas.";
            return;
        }

        Buscando = true;
        try
        {
            var res = await productoService.BuscarAsync(texto);
            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudo buscar el producto.");
                return;
            }

            Resultados.Clear();
            foreach (var producto in res.productos ?? new List<ProductoEntidad>())
            {
                Resultados.Add(producto);
            }

            BusquedaRealizada = true;
            OnPropertyChanged(nameof(SinResultados));
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            Buscando = false;
        }
    }

    [RelayCommand]
    private async Task AgregarProductoAsync(ProductoEntidad producto)
    {
        if (EstaOcupado || producto is null) return;

        await EjecutarAsync(async () =>
        {
            var res = await itemService.AgregarAsync(_guidLista, producto.guidProducto, 1);
            if (!res.resultado) return MensajesError.Traducir(res.error, "No se pudo agregar el producto.");

            BuscadorVisible = false;
            TextoBusqueda = string.Empty;
            Resultados.Clear();
            BusquedaRealizada = false;
            return null;
        });
    }

    [RelayCommand]
    private Task IncrementarAsync(ItemLista item) => CambiarCantidadAsync(item, 1);

    [RelayCommand]
    private Task DecrementarAsync(ItemLista item) => CambiarCantidadAsync(item, -1);

    private async Task CambiarCantidadAsync(ItemLista item, decimal delta)
    {
        if (EstaOcupado || item is null) return;

        decimal nueva = item.cantidad + delta;
        if (nueva < 1) return;

        await EjecutarAsync(async () =>
        {
            var res = await itemService.ActualizarAsync(item, nueva, item.completado);
            return res.resultado ? null : MensajesError.Traducir(res.error, "No se pudo cambiar la cantidad.");
        });
    }

    [RelayCommand]
    private async Task AlternarCompletadoAsync(ItemLista item)
    {
        if (EstaOcupado || item is null) return;

        await EjecutarAsync(async () =>
        {
            var res = await itemService.ActualizarAsync(item, item.cantidad, !item.completado);
            return res.resultado ? null : MensajesError.Traducir(res.error, "No se pudo actualizar el producto.");
        });
    }

    [RelayCommand]
    private async Task EliminarItemAsync(ItemLista item)
    {
        if (EstaOcupado || item is null) return;

        bool confirmar = await Shell.Current.DisplayAlertAsync(
            "Quitar producto",
            $"¿Quitar «{item.nombreProducto}» de la lista?",
            "Sí, quitar",
            "Cancelar");
        if (!confirmar) return;

        await EjecutarAsync(async () =>
        {
            var res = await itemService.EliminarAsync(item.guidItem);
            return res.resultado ? null : MensajesError.Traducir(res.error, "No se pudo quitar el producto.");
        });
    }

    [RelayCommand]
    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private async Task RecargarItemsAsync()
    {
        var res = await itemService.ObtenerAsync(_guidLista);
        if (!res.resultado)
        {
            MensajeError = MensajesError.Traducir(res.error, "No se pudieron cargar los productos.");
            return;
        }

        Items.Clear();
        foreach (var item in res.Items ?? new List<ItemLista>())
        {
            Items.Add(item);
        }

        OnPropertyChanged(nameof(ListaVacia));
    }

    // Corre una accion que devuelve el mensaje de error (o null si salio bien) y, si salio bien,
    // recarga los items para mostrar el estado real del servidor.
    private async Task EjecutarAsync(Func<Task<string?>> accion)
    {
        EstaOcupado = true;
        MensajeError = string.Empty;
        try
        {
            string? error = await accion();
            if (error is not null)
            {
                MensajeError = error;
                return;
            }

            await RecargarItemsAsync();
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            EstaOcupado = false;
        }
    }
}
