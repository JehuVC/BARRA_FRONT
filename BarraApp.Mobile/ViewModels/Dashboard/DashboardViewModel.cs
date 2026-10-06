using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Historial;
using BarraApp.Mobile.Services.Sesion;
using BarraApp.Mobile.Views.Comercio;
using BarraApp.Mobile.Views.Favorito;
using BarraApp.Mobile.Views.Guild;
using BarraApp.Mobile.Views.Historial;
using BarraApp.Mobile.Views.ListaCompra;
using BarraApp.Mobile.Views.Pago;
using BarraApp.Mobile.Views.Producto;
using BarraApp.Mobile.Views.Usuario;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entidades.Enti_Clases;

namespace BarraApp.Mobile.ViewModels.Dashboard;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IHistorialService _historialService;
    private readonly ISesionService _sesionService;
    private readonly IAuthTokenStore _tokenStore;

    [ObservableProperty] private string nombreUsuario = "Usuario";
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

    public DashboardViewModel(IHistorialService historialService, ISesionService sesionService, IAuthTokenStore tokenStore)
    {
        _historialService = historialService;
        _sesionService = sesionService;
        _tokenStore = tokenStore;
    }

    // Reemplaza el nombre de ejemplo por el del usuario autenticado.
    [RelayCommand]
    public async Task CargarNombreAsync()
    {
        try
        {
            var res = await _sesionService.ObtenerPerfilAsync();
            if (res.resultado && res.usuario is not null && !string.IsNullOrWhiteSpace(res.usuario.nombre))
            {
                NombreUsuario = res.usuario.nombre;
            }
        }
        catch (Exception)
        {
            // Si falla se deja el nombre por defecto; el mensaje de sesion expirada ya lo maneja AuthHeaderHandler.
            // Se atrapa todo (no solo ApiException): este comando se lanza sin await desde OnAppearing y una
            // excepcion sin atrapar cierra la app.
        }
    }

    [RelayCommand]
    private async Task CambiarPestana(string pestana)
    {
        // Listas y Gremios son pantallas propias (se abren encima, igual que Perfil); las demas
        // pestañas siguen siendo contenido de esta misma pagina.
        if (pestana == "Listas")
        {
            await Shell.Current.GoToAsync(nameof(ListasPage));
            return;
        }

        if (pestana == "Gremios")
        {
            await Shell.Current.GoToAsync(nameof(GuildPage));
            return;
        }

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

        if (opcion == "Favoritos")
        {
            await Shell.Current.GoToAsync(nameof(FavoritosPage));
            return;
        }

        if (opcion == "Historial")
        {
            await Shell.Current.GoToAsync(nameof(HistorialPage));
            return;
        }

        if (opcion == "Comercios")
        {
            await Shell.Current.GoToAsync(nameof(ComerciosPage));
            return;
        }

        if (opcion == "Pagos")
        {
            await Shell.Current.GoToAsync(nameof(PagoPage));
            return;
        }

        if (opcion == "RegistrarProducto")
        {
            await Shell.Current.GoToAsync(nameof(ProductoFormPage));
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

            // Antes solo navegaba al login: la sesion seguia viva en el servidor y el JWT guardado
            // en el telefono. Ahora se cierra en el backend (revoca el JWT) y se borra el token local.
            try
            {
                await _sesionService.LogoutAsync();
            }
            catch (ApiException)
            {
                // Sin conexion o JWT ya vencido: igual se borra la sesion local.
            }

            await _tokenStore.LimpiarTokenAsync();

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
        await Shell.Current.GoToAsync(nameof(ScannerPage));
    }

    [RelayCommand]
    private async Task BuscarTextoAsync(string? texto)
    {
        // Con texto abre la busqueda ya ejecutada; sin texto (o desde una busqueda reciente que solo
        // tiene producto) abre el buscador vacio.
        string ruta = string.IsNullOrWhiteSpace(texto)
            ? nameof(BusquedaPage)
            : $"{nameof(BusquedaPage)}?texto={Uri.EscapeDataString(texto.Trim())}";

        await Shell.Current.GoToAsync(ruta);
    }
}