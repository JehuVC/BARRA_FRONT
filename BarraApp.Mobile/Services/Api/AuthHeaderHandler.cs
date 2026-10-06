using System.Net;
using System.Net.Http.Headers;

namespace BarraApp.Mobile.Services.Api;


public class AuthHeaderHandler(IAuthTokenStore tokenStore) : DelegatingHandler
{
    private static int _redirigiendoALogin;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenStore.ObtenerTokenAsync();
        bool teniaToken = !string.IsNullOrWhiteSpace(token);
        if (teniaToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // El backend responde 401 solo cuando el JWT esta vencido, es invalido o la sesion
        // fue cerrada (login responde 200 aunque la clave sea mala): el token guardado ya no
        // sirve, asi que se borra y se manda al usuario a iniciar sesion de nuevo.
        if (teniaToken && response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await SesionExpiradaAsync();
        }

        return response;
    }

    private async Task SesionExpiradaAsync()
    {
        await tokenStore.LimpiarTokenAsync();

        // Varias peticiones en paralelo pueden recibir 401 a la vez; solo la primera navega.
        if (Interlocked.Exchange(ref _redirigiendoALogin, 1) == 1)
        {
            return;
        }

        // La navegacion NO se espera: la peticion tiene que terminar ya (el llamador recibe "Tu sesion expiro") y
        // navegar en medio de otra pantalla que aun esta apareciendo puede fallar. Cualquier error al navegar se
        // ignora a proposito: no debe tumbar la app (el token ya esta borrado, el siguiente intento va a Login).
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                if (Shell.Current is null) return;

                bool yaEnLogin = Shell.Current.CurrentState?.Location?.ToString()?.Contains("LoginPage") == true;
                if (yaEnLogin) return;

                await Shell.Current.GoToAsync("//LoginPage");
                await Shell.Current.DisplayAlertAsync("Sesión expirada", "Tu sesión terminó. Inicia sesión de nuevo.", "OK");
            }
            catch (Exception)
            {
            }
            finally
            {
                Interlocked.Exchange(ref _redirigiendoALogin, 0);
            }
        });
    }
}
