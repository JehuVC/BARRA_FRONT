using System;
using System.Linq;
using System.Threading.Tasks;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Sesion;
using BarraApp.Mobile.Views.Usuario;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DTO.Usuario;

namespace BarraApp.Mobile.ViewModels.Usuario;

public partial class PerfilViewModel(ISesionService sesionService, IAuthTokenStore tokenStore) : ObservableObject
{
    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private string correo = string.Empty;
    [ObservableProperty] private string zona = string.Empty;
    [ObservableProperty] private string rol = string.Empty;
    [ObservableProperty] private string nombreGuild = string.Empty;
    [ObservableProperty] private bool estaEditando;
    [ObservableProperty] private string mensajeError = string.Empty;
    [ObservableProperty] private string mensajeExito = string.Empty;
    [ObservableProperty] private bool estaOcupado;

    private string _nombreOriginal = string.Empty;
    private string _zonaOriginal = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool HasSuccess => !string.IsNullOrWhiteSpace(MensajeExito);
    public bool PuedeInteractuar => !EstaOcupado;
    public bool NoEstaEditando => !EstaEditando;

    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));
    partial void OnMensajeExitoChanged(string value) => OnPropertyChanged(nameof(HasSuccess));
    partial void OnEstaOcupadoChanged(bool value) => OnPropertyChanged(nameof(PuedeInteractuar));
    partial void OnEstaEditandoChanged(bool value) => OnPropertyChanged(nameof(NoEstaEditando));

    [RelayCommand]
    public async Task CargarPerfilAsync()
    {
        if (EstaOcupado) return;

        EstaOcupado = true;
        MensajeError = string.Empty;
        MensajeExito = string.Empty;

        try
        {
            var res = await sesionService.ObtenerPerfilAsync();

            if (!res.resultado || res.usuario is null)
            {
                MensajeError = res.error?.FirstOrDefault()?.mensaje ?? "No se pudo obtener el perfil.";
                return;
            }

            Nombre = res.usuario.nombre ?? string.Empty;
            Correo = res.usuario.correo ?? string.Empty;
            Zona = res.usuario.zona ?? string.Empty;
            Rol = res.usuario.rolGuild ?? "Miembro";
            NombreGuild = res.usuario.nombreGuild ?? "Sin Gremio";

            _nombreOriginal = Nombre;
            _zonaOriginal = Zona;
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

    [RelayCommand]
    private void HabilitarEdicion()
    {
        _nombreOriginal = Nombre;
        _zonaOriginal = Zona;
        EstaEditando = true;
        MensajeError = string.Empty;
        MensajeExito = string.Empty;
    }

    [RelayCommand]
    private void CancelarEdicion()
    {
        Nombre = _nombreOriginal;
        Zona = _zonaOriginal;
        EstaEditando = false;
        MensajeError = string.Empty;
    }

    [RelayCommand]
    private async Task GuardarCambiosAsync()
    {
        if (EstaOcupado) return;

        if (string.IsNullOrWhiteSpace(Nombre))
        {
            MensajeError = "El nombre no puede estar vacío.";
            return;
        }

        // 1. DIÁLOGO DE CONFIRMACIÓN (Sí / No)
        bool deseaCambiar = await Shell.Current.DisplayAlert(
            "Confirmar Actualización",
            "¿Deseas guardar los cambios en tu perfil?",
            "Sí, guardar",
            "No, cancelar");

        if (!deseaCambiar)
        {
            return;
        }

        EstaOcupado = true;
        MensajeError = string.Empty;
        MensajeExito = string.Empty;

        try
        {
            var dto = new DtoActualizarUsuario
            {
                Nombre = Nombre.Trim(),
                Zona = Zona?.Trim() ?? string.Empty
            };

            var res = await sesionService.ActualizarPerfilAsync(dto);

            if (!res.resultado)
            {
                MensajeError = res.error?.FirstOrDefault()?.mensaje ?? "No se pudieron guardar los cambios.";
                return;
            }

            _nombreOriginal = Nombre;
            _zonaOriginal = Zona;
            EstaEditando = false;
            MensajeExito = "¡Perfil actualizado con éxito!";
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

    [RelayCommand]
    private void CerrarNotificacion()
    {
        MensajeExito = string.Empty;
        MensajeError = string.Empty;
    }

    [RelayCommand]
    private async Task CerrarSesionAsync()
    {
        if (EstaOcupado) return;

        bool confirmar = await Shell.Current.DisplayAlert(
            "Cerrar Sesión",
            "¿Estás seguro de que deseas salir?",
            "Sí, salir",
            "Cancelar");

        if (!confirmar) return;

        EstaOcupado = true;
        try
        {
            try
            {
                await sesionService.LogoutAsync();
            }
            catch { }

            await tokenStore.GuardarTokenAsync(string.Empty);

            // Notificación visual de despedida
            await Shell.Current.DisplayAlert(
                "👋 ¡Hasta pronto!",
                "Tu sesión se ha cerrado correctamente.",
                "Entendido");

            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    [RelayCommand]
    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}