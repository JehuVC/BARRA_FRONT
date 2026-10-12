using System.Collections.ObjectModel;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Comercio;
using BarraApp.Mobile.Services.Ubicacion;
using BarraApp.Mobile.Views.Comercio;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BarraApp.Mobile.ViewModels.Comercio;

// Comercios cerca del usuario (api/comercio/obtenerCercanos) con radio configurable. Desde cada uno se puede
// abrir el mapa, llamar y editar sus datos; tambien se puede registrar un comercio nuevo.
public partial class ComerciosViewModel(IComercioService comercioService, IUbicacionService ubicacionService) : ObservableObject
{
    private const decimal RadioPorDefectoKm = 5;
    private static readonly decimal[] RadiosKm = { 1, 3, 5, 10, 25 };

    // Enlazada al RefreshView: true mientras carga (primera carga y "jalar para recargar").
    [ObservableProperty] private bool cargando;
    [ObservableProperty] private bool cargaInicialTerminada;
    [ObservableProperty] private bool sinUbicacion;
    [ObservableProperty] private string mensajeError = string.Empty;

    public ObservableCollection<RadioChipViewModel> Radios { get; } =
        new(RadiosKm.Select(r => new RadioChipViewModel(r) { Seleccionado = r == RadioPorDefectoKm }));

    public ObservableCollection<ComercioItemViewModel> Comercios { get; } = new();

    private decimal RadioActual => Radios.FirstOrDefault(r => r.Seleccionado)?.Km ?? RadioPorDefectoKm;

    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool HayComercios => Comercios.Count > 0;
    public bool SinResultados => CargaInicialTerminada && !Cargando && !SinUbicacion && !HasError && Comercios.Count == 0;
    public bool PuedeAmpliar => Radios.Any(r => r.Km > RadioActual);
    public string Resumen => Comercios.Count == 1
        ? $"1 comercio a menos de {RadioActual:0.##} km"
        : $"{Comercios.Count} comercios a menos de {RadioActual:0.##} km";

    partial void OnMensajeErrorChanged(string value)
    {
        OnPropertyChanged(nameof(HasError));
        OnPropertyChanged(nameof(SinResultados));
    }

    partial void OnCargaInicialTerminadaChanged(bool value) => OnPropertyChanged(nameof(SinResultados));
    partial void OnCargandoChanged(bool value) => OnPropertyChanged(nameof(SinResultados));
    partial void OnSinUbicacionChanged(bool value) => OnPropertyChanged(nameof(SinResultados));

    [RelayCommand]
    public async Task CargarAsync()
    {
        Cargando = true;
        MensajeError = string.Empty;

        try
        {
            // Sin mostrar el aviso de permisos: eso solo pasa si el usuario toca "Activar ubicación".
            var ubicacion = await ubicacionService.ObtenerAsync(pedirPermiso: false);
            SinUbicacion = ubicacion is null;

            if (ubicacion is not { } u)
            {
                Comercios.Clear();
                return;
            }

            var res = await comercioService.ObtenerCercanosAsync(u.Latitud, u.Longitud, RadioActual);
            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudieron cargar los comercios.");
                return;
            }

            Comercios.Clear();
            foreach (var c in (res.comercios ?? new()).OrderBy(c => c.distanciaKm ?? double.MaxValue))
            {
                Comercios.Add(new ComercioItemViewModel(c));
            }
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            CargaInicialTerminada = true;
            Cargando = false;
            NotificarLista();
        }
    }

    [RelayCommand]
    private async Task ActivarUbicacionAsync()
    {
        var ubicacion = await ubicacionService.ObtenerAsync(pedirPermiso: true);
        if (ubicacion is null)
        {
            bool abrir = await Shell.Current.DisplayAlertAsync(
                "No pudimos obtener tu ubicación",
                "Activa el GPS y permite el acceso a la ubicación en los ajustes de la aplicación.",
                "Abrir ajustes",
                "Cancelar");

            if (abrir)
            {
                AppInfo.Current.ShowSettingsUI();
            }

            return;
        }

        await CargarAsync();
    }

    [RelayCommand]
    private async Task SeleccionarRadioAsync(RadioChipViewModel chip)
    {
        if (chip is null || chip.Seleccionado || Cargando) return;

        foreach (var r in Radios)
        {
            r.Seleccionado = ReferenceEquals(r, chip);
        }

        await CargarAsync();
    }

    [RelayCommand]
    private async Task AmpliarRadioAsync()
    {
        var siguiente = Radios.FirstOrDefault(r => r.Km > RadioActual);
        if (siguiente is not null)
        {
            await SeleccionarRadioAsync(siguiente);
        }
    }

    [RelayCommand]
    private async Task NuevoComercioAsync()
    {
        await Shell.Current.GoToAsync(nameof(ComercioFormPage));
    }

    // Se pasa la entidad completa: el backend no tiene "obtener un comercio por id".
    [RelayCommand]
    private async Task EditarAsync(ComercioItemViewModel item)
    {
        if (item is null) return;

        await Shell.Current.GoToAsync(nameof(ComercioFormPage),
            new ShellNavigationQueryParameters { { "comercio", item.Comercio } });
    }

    [RelayCommand]
    private async Task AbrirMapaAsync(ComercioItemViewModel item)
    {
        if (item is null) return;

        try
        {
            await Map.Default.OpenAsync(
                (double)item.Comercio.latitud,
                (double)item.Comercio.longitud,
                new MapLaunchOptions { Name = item.Nombre });
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlertAsync("No se pudo abrir el mapa", "No encontramos una aplicación de mapas en este dispositivo.", "OK");
        }
    }

    [RelayCommand]
    private async Task LlamarAsync(ComercioItemViewModel item)
    {
        if (item is null || !item.TieneTelefono) return;

        try
        {
            PhoneDialer.Default.Open(item.Comercio.telefono);
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlertAsync("No se pudo llamar", "Este dispositivo no puede hacer llamadas.", "OK");
        }
    }

    [RelayCommand]
    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void NotificarLista()
    {
        OnPropertyChanged(nameof(HayComercios));
        OnPropertyChanged(nameof(SinResultados));
        OnPropertyChanged(nameof(PuedeAmpliar));
        OnPropertyChanged(nameof(Resumen));
    }
}