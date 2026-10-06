namespace BarraApp.Mobile.Services.Ubicacion;

public interface IUbicacionService
{
    // Ubicacion actual. Con pedirPermiso = false NO muestra el aviso de permisos: devuelve null si el usuario
    // todavia no lo concedio. Con true lo pide. Tambien devuelve null si se nego o el GPS no responde.
    Task<(decimal Latitud, decimal Longitud)?> ObtenerAsync(bool pedirPermiso);
}
