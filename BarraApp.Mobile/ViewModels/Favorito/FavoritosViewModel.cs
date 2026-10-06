using System.Collections.ObjectModel;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Favorito;
using BarraApp.Mobile.Services.ListaCompra;
using BarraApp.Mobile.Views.Producto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BarraApp.Mobile.ViewModels.Favorito;

// Lista de productos favoritos (api/favorito/obtener). Desde cada uno se puede quitar de favoritos
// (api/favorito/quitar, con "Deshacer"), agregarlo a una lista de compra o abrir su detalle.
public partial class FavoritosViewModel(IFavoritoService favoritoService, IAgregarAListaFlujo agregarAListaFlujo) : ObservableObject
{
    private FavoritoItemViewModel? _ultimoQuitado;

    // Enlazada al RefreshView: true mientras carga (primera carga y "jalar para recargar").
    [ObservableProperty] private bool cargando;
    [ObservableProperty] private bool cargaInicialTerminada;
    [ObservableProperty] private string mensajeError = string.Empty;
    [ObservableProperty] private string mensajeExito = string.Empty;
    [ObservableProperty] private bool puedeDeshacer;

    public ObservableCollection<FavoritoItemViewModel> Favoritos { get; } = new();

    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool HasSuccess => !string.IsNullOrWhiteSpace(MensajeExito);
    public bool HayFavoritos => Favoritos.Count > 0;
    public bool SinFavoritos => CargaInicialTerminada && !Cargando && Favoritos.Count == 0;
    public string Resumen => Favoritos.Count == 1 ? "1 producto" : $"{Favoritos.Count} productos";

    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));
    partial void OnMensajeExitoChanged(string value) => OnPropertyChanged(nameof(HasSuccess));
    partial void OnCargaInicialTerminadaChanged(bool value) => OnPropertyChanged(nameof(SinFavoritos));
    partial void OnCargandoChanged(bool value) => OnPropertyChanged(nameof(SinFavoritos));

    [RelayCommand]
    public async Task CargarAsync()
    {
        Cargando = true;
        MensajeError = string.Empty;

        try
        {
            var res = await favoritoService.ObtenerAsync();
            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudieron cargar tus favoritos.");
                return;
            }

            // Los mas recientes primero.
            var ordenados = (res.favoritos ?? new())
                .OrderByDescending(f => f.fechaAgregado ?? DateTime.MinValue)
                .Select(f => new FavoritoItemViewModel(f));

            Favoritos.Clear();
            foreach (var item in ordenados)
            {
                Favoritos.Add(item);
            }

            NotificarLista();
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            CargaInicialTerminada = true;
            Cargando = false;
        }
    }

    [RelayCommand]
    private async Task QuitarAsync(FavoritoItemViewModel item)
    {
        if (item is null || item.Procesando) return;

        item.Procesando = true;
        MensajeError = string.Empty;
        try
        {
            var res = await favoritoService.QuitarAsync(item.GuidProducto);
            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudo quitar de favoritos.");
                return;
            }

            Favoritos.Remove(item);
            NotificarLista();
            MostrarConDeshacer(item);
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            item.Procesando = false;
        }
    }

    // Vuelve a marcar como favorito el ultimo producto quitado y recarga la lista.
    [RelayCommand]
    private async Task DeshacerAsync()
    {
        var item = _ultimoQuitado;
        if (item is null) return;

        _ultimoQuitado = null;
        PuedeDeshacer = false;
        MensajeExito = string.Empty;
        MensajeError = string.Empty;

        try
        {
            var res = await favoritoService.AgregarAsync(item.GuidProducto);
            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudo volver a agregar a favoritos.");
                return;
            }

            await CargarAsync();
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
    }

    [RelayCommand]
    private async Task AgregarAListaAsync(FavoritoItemViewModel item)
    {
        if (item is null || item.Procesando) return;

        item.Procesando = true;
        MensajeError = string.Empty;
        try
        {
            var r = await agregarAListaFlujo.EjecutarAsync(item.GuidProducto, item.Nombre);
            if (r.Error is not null)
            {
                MensajeError = r.Error;
            }
            else if (r.Exito is not null)
            {
                _ultimoQuitado = null;
                PuedeDeshacer = false;
                MostrarExito(r.Exito);
            }
        }
        finally
        {
            item.Procesando = false;
        }
    }

    [RelayCommand]
    private async Task AbrirDetalleAsync(FavoritoItemViewModel item)
    {
        if (item is null || string.IsNullOrWhiteSpace(item.CodigoBarras)) return;

        await Shell.Current.GoToAsync(
            $"{nameof(ProductoDetallePage)}?codigo={Uri.EscapeDataString(item.CodigoBarras)}");
    }

    [RelayCommand]
    private async Task BuscarProductosAsync()
    {
        await Shell.Current.GoToAsync(nameof(BusquedaPage));
    }

    [RelayCommand]
    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void NotificarLista()
    {
        OnPropertyChanged(nameof(HayFavoritos));
        OnPropertyChanged(nameof(SinFavoritos));
        OnPropertyChanged(nameof(Resumen));
    }

    // Aviso con boton "Deshacer" que se oculta solo a los 6 segundos.
    private async void MostrarConDeshacer(FavoritoItemViewModel item)
    {
        _ultimoQuitado = item;
        MensajeExito = $"«{item.Nombre}» ya no está en tus favoritos.";
        PuedeDeshacer = true;

        await Task.Delay(6000);
        if (ReferenceEquals(_ultimoQuitado, item))
        {
            _ultimoQuitado = null;
            PuedeDeshacer = false;
            MensajeExito = string.Empty;
        }
    }

    // Aviso breve que se oculta solo.
    private async void MostrarExito(string mensaje)
    {
        MensajeExito = mensaje;
        await Task.Delay(2500);
        if (MensajeExito == mensaje)
        {
            MensajeExito = string.Empty;
        }
    }
}
