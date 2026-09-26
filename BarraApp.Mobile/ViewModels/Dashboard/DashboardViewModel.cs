using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BarraApp.Mobile.Services.Historial;
using BarraApp.Mobile.Views.Usuario;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entidades.Enti_Clases;

namespace BarraApp.Mobile.ViewModels.Dashboard;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IHistorialService _historialService;

    [ObservableProperty] private string nombreUsuario = "Néstor";
    [ObservableProperty] private bool estaCargando;
    [ObservableProperty] private bool noHayBusquedas;

    // Controladores de Pestañas Inferiores (Módulos de alto uso)
    [ObservableProperty] private bool verInicio = true;
    [ObservableProperty] private bool verListas;
    [ObservableProperty] private bool verComunidad;
    [ObservableProperty] private bool verGremios;

    // Controlador del Menú Lateral (Módulos secundarios)
    [ObservableProperty] private bool menuLateralVisible;

    public ObservableCollection<HistorialBusqueda> BusquedasRecientes { get; } = new();

    public DashboardViewModel(IHistorialService historialService)
    {
        _historialService = historialService;
    }

    [RelayCommand]
    private void CambiarPestana(string pestana)
    {
        VerInicio = pestana == "Inicio";
        VerListas = pestana == "Listas";
        VerComunidad = pestana == "Comunidad";
        VerGremios = pestana == "Gremios";
    }

    [RelayCommand]
    private void ToggleMenuLateral()
    {
        MenuLateralVisible = !MenuLateralVisible;
    }

    [RelayCommand]
    private async Task OpcionMenuAsync(string opcion)
    {
        MenuLateralVisible = false; // Cierra el menú de forma explícita

        if (opcion == "Perfil")
        {
            await Shell.Current.GoToAsync(nameof(PerfilPage));
            return;
        }

        if (opcion == "CerrarSesion")
        {
            bool confirmar = await Shell.Current.DisplayAlert(
                "Cerrar Sesión",
                "¿Estás seguro de que deseas salir?",
                "Sí, salir",
                "Cancelar");

            if (!confirmar) return;

            await Shell.Current.DisplayAlert(
                "👋 ¡Hasta pronto!",
                "Tu sesión se ha cerrado correctamente.",
                "Entendido");

            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }

        await Shell.Current.DisplayAlert("Módulo", $"Abriendo: {opcion}\n(Próximo paso a programar)", "OK");
    }

    [RelayCommand]
    public async Task CargarHistorialAsync()
    {
        if (EstaCargando) return;
        EstaCargando = true;
        NoHayBusquedas = false;

        try
        {
            var res = await _historialService.ObtenerHistorialAsync();
            BusquedasRecientes.Clear();

            if (res.resultado && res.historial != null && res.historial.Count > 0)
            {
                foreach (var item in res.historial)
                {
                    BusquedasRecientes.Add(item);
                }
            }
            else
            {
                NoHayBusquedas = true;
            }
        }
        catch (Exception)
        {
            NoHayBusquedas = true;
        }
        finally
        {
            EstaCargando = false;
        }
    }

    [RelayCommand]
    private async Task EscanearAsync()
    {
        await Shell.Current.DisplayAlert("Escáner", "Abriendo cámara (Módulo 2)...", "OK");
    }

    [RelayCommand]
    private async Task BuscarTextoAsync(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return;
        await Shell.Current.DisplayAlert("Búsqueda", $"Buscando: {texto}", "OK");
    }
}