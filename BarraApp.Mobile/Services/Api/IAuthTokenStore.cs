namespace BarraApp.Mobile.Services.Api;


public interface IAuthTokenStore
{
    Task<string?> ObtenerTokenAsync();
    Task GuardarTokenAsync(string token);
    Task LimpiarTokenAsync();
}
