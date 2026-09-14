using System.Net.Http.Headers;

namespace BarraApp.Mobile.Services.Api;

// Le agrega el header Authorization: Bearer <token> a cada request saliente,
// si hay una sesion guardada. Asi ningun servicio (SesionService, GuildService,
// etc.) tiene que acordarse de hacerlo a mano.
public class AuthHeaderHandler(IAuthTokenStore tokenStore) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenStore.ObtenerTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
