using System.Collections.ObjectModel;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Historial;
using BarraApp.Mobile.Views.Producto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BarraApp.Mobile.ViewModels.Historial;

// Historial de busquedas (api/historial/obtenerPorUsuario): agrupado por dia y sin repetidas. Tocar una entrada
// vuelve a hacer esa busqueda. El backend devuelve solo las ultimas 20 y no permite borrar.
public partial class HistorialViewModel(IHistorialService historialService) : ObservableObject
{
    // Enlazada al RefreshView: true mientras carga (primera carga y "jalar para recargar").
    [ObservableProperty] private bool cargando;
    [ObservableProperty] private bool cargaInicialTerminada;
    [ObservableProperty] private string mensajeError = string.Empty;

    public ObservableCollection<GrupoHistorial> Grupos { get; } = new();

    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool HayHistorial => Grupos.Count > 0;
    public bool SinHistorial => CargaInicialTerminada && !Cargando && Grupos.Count == 0;
    public string Resumen
    {
        get
        {
            int total = Grupos.Sum(g => g.Count);
            return total == 1 ? "1 búsqueda" : $"{total} búsquedas";
        }
    }

    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));
    partial void OnCargaInicialTerminadaChanged(bool value) => OnPropertyChanged(nameof(SinHistorial));
    partial void OnCargandoChanged(bool value) => OnPropertyChanged(nameof(SinHistorial));

    [RelayCommand]
    public async Task CargarAsync()
    {
        Cargando = true;
        MensajeError = string.Empty;

        try
        {
            var res = await historialService.ObtenerHistorialAsync();
            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudo cargar tu historial.");
                return;
            }

            Grupos.Clear();
            foreach (var grupo in HistorialAgrupador.Agrupar(res.historial, DateTime.UtcNow))
            {
                Grupos.Add(grupo);
            }

            OnPropertyChanged(nameof(HayHistorial));
            OnPropertyChanged(nameof(SinHistorial));
            OnPropertyChanged(nameof(Resumen));
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
    private async Task AbrirAsync(HistorialItemViewModel item)
    {
        if (item is null) return;

        await Shell.Current.GoToAsync($"{nameof(BusquedaPage)}?texto={Uri.EscapeDataString(item.Texto)}");
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
}
