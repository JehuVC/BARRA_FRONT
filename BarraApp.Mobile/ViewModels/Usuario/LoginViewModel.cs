using System.Linq;
using System.Threading.Tasks;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Sesion;
using BarraApp.Mobile.Views.Dashboard;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BarraApp.Mobile.Views.Usuario;

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
                MensajeError = MensajesError.Traducir(res.error, "No se pudo iniciar sesión. Intenta de nuevo.");
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

    // Sesion recordada (FE-S6-03): si ya hay un JWT guardado se salta el login. Si ese token
    // ya vencio o fue revocado, la primera llamada protegida responde 401 y AuthHeaderHandler
    // lo borra y devuelve al usuario aqui.
    [RelayCommand]
    public async Task InicializarAsync()
    {
        try
        {
            var token = await tokenStore.ObtenerTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
            {
                await IrAlDashboardCuandoShellEsteListoAsync();
            }
        }
        catch (Exception)
        {
            // SecureStorage puede fallar (ej. cambio de bloqueo del dispositivo): se pide login normal.
        }
    }

    // En Windows OnAppearing de la primera pagina corre mientras el Shell todavia se esta armando; navegar en ese
    // momento hace que el propio Shell lance "Pending Navigations still processing" (una excepcion interna que no
    // se puede atrapar y cierra la app). Por eso se espera un poco y, si el Shell aun esta ocupado, se reintenta.
    private static async Task IrAlDashboardCuandoShellEsteListoAsync()
    {
        await Task.Delay(300);

        for (int intento = 0; intento < 5; intento++)
        {
            try
            {
                await Shell.Current.GoToAsync($"//{nameof(DashboardPage)}");
                return;
            }
            catch (InvalidOperationException)
            {
                await Task.Delay(300);
            }
        }
    }

    [RelayCommand]
    private async Task IrARegistroAsync()
    {
        await Shell.Current.GoToAsync(nameof(RegistroPage));
    }

    [RelayCommand]
    private async Task IrARecuperarAsync()
    {
        await Shell.Current.GoToAsync(nameof(RecuperarPage));
    }

}
