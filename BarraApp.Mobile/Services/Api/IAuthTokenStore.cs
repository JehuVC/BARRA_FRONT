namespace BarraApp.Mobile.Services.Api;

// Guarda/lee el JWT de sesion. Implementado sobre SecureStorage (FE-S6-03)
// para que sobreviva a cerrar la app sin tener que iniciar sesion de nuevo.
public interface IAuthTokenStore
{
    Task<string?> ObtenerTokenAsync();
    Task GuardarTokenAsync(string token);
    Task LimpiarTokenAsync();
}
