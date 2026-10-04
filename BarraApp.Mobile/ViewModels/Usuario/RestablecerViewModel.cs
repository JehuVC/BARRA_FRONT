using BarraApp.Mobile.Helpers;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Sesion;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BarraApp.Mobile.ViewModels.Usuario;

// FE-S7-05: codigo recibido por correo + contrasena nueva.
public partial class RestablecerViewModel(ISesionService sesionService) : ObservableObject, IQueryAttributable
{
    [ObservableProperty] private string correo = string.Empty;
    [ObservableProperty] private string codigo = string.Empty;
    [ObservableProperty] private string nuevaClave = string.Empty;
    [ObservableProperty] private string confirmarClave = string.Empty;
    [ObservableProperty] private string mensajeError = string.Empty;
    [ObservableProperty] private bool estaOcupado;

    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool PuedeEnviar => !EstaOcupado;

    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));
    partial void OnEstaOcupadoChanged(bool value) => OnPropertyChanged(nameof(PuedeEnviar));

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("correo", out var valor))
        {
            Correo = Uri.UnescapeDataString(valor?.ToString() ?? string.Empty);
        }
    }

    [RelayCommand]
    private async Task CambiarClaveAsync()
    {
        if (EstaOcupado) return;
        MensajeError = string.Empty;

        string codigoLimpio = Codigo.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(codigoLimpio))
        {
            MensajeError = "Ingresa el código que recibiste por correo.";
            return;
        }

        if (!ValidacionCliente.EsPasswordValida(NuevaClave))
        {
            MensajeError = ValidacionCliente.ReglasPassword;
            return;
        }

        if (NuevaClave != ConfirmarClave)
        {
            MensajeError = "Las contraseñas no coinciden.";
            return;
        }

        EstaOcupado = true;
        try
        {
            // 1. Valida el codigo por separado para dar un error claro antes de tocar la clave.
            var validacion = await sesionService.ValidarCodigoRecuperacionAsync(Correo, codigoLimpio);
            if (!validacion.resultado)
            {
                MensajeError = MensajesError.Traducir(validacion.error);
                return;
            }

            // 2. El backend vuelve a validar el codigo al restablecer, no confia en el paso anterior.
            var res = await sesionService.RestablecerClaveAsync(Correo, codigoLimpio, NuevaClave);
            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error);
                return;
            }

            await Shell.Current.DisplayAlertAsync("Contraseña actualizada", "Ya puedes iniciar sesión con tu nueva contraseña.", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
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
    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
