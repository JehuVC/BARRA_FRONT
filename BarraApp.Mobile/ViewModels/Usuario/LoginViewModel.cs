using System.Linq;
using System.Threading.Tasks;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Sesion;
using BarraApp.Mobile.Views.Dashboard;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BarraApp.Mobile.ViewModels.Usuario;

public partial class LoginViewModel(ISesionService sesionService, IAuthTokenStore tokenStore) : ObservableObject
{
    [ObservableProperty]
    private string correo = string.Empty;

    [ObservableProperty]
    private string clave = string.Empty;

    [ObservableProperty]
    private string mensajeError = string.Empty;

    [ObservableProperty]
    private bool estaOcupado;

    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);

    public bool PuedeEnviar => !EstaOcupado;

    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));

    partial void OnEstaOcupadoChanged(bool value) => OnPropertyChanged(nameof(PuedeEnviar));

    [RelayCommand]
    private async Task IniciarSesionAsync()
    {
        if (EstaOcupado)
        {
            return;
        }

        MensajeError = string.Empty;

        if (string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(Clave))
        {
            MensajeError = "Ingresa tu correo y tu contrasena.";
            return;
        }

        EstaOcupado = true;
        try
        {
            var res = await sesionService.LoginAsync(Correo.Trim(), Clave);

            if (!res.resultado || res.sesion is null)
            {
                MensajeError = TraducirError(res.error?.FirstOrDefault()?.mensaje);
                return;
            }

            await tokenStore.GuardarTokenAsync(res.sesion.tokenJwt);
            await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
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

    private static string TraducirError(string? codigo) => codigo switch
    {
        "credencialesInvalidas" => "Correo o contrasena incorrectos.",
        "usuarioInactivo" => "Esta cuenta esta inactiva. Contacta a soporte.",
        _ => "No se pudo iniciar sesion. Intenta de nuevo."
    };
}
