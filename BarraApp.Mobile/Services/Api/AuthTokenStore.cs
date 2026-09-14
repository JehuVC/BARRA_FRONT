namespace BarraApp.Mobile.Services.Api;

public class AuthTokenStore : IAuthTokenStore
{
    private const string ClaveToken = "jwt_token";

    public Task<string?> ObtenerTokenAsync() => SecureStorage.Default.GetAsync(ClaveToken);

    public Task GuardarTokenAsync(string token) => SecureStorage.Default.SetAsync(ClaveToken, token);

    public Task LimpiarTokenAsync()
    {
        SecureStorage.Default.Remove(ClaveToken);
        return Task.CompletedTask;
    }
}
