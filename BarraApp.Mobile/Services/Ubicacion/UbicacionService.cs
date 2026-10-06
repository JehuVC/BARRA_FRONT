namespace BarraApp.Mobile.Services.Ubicacion;

public class UbicacionService : IUbicacionService
{
    public async Task<(decimal Latitud, decimal Longitud)?> ObtenerAsync(bool pedirPermiso)
    {
        try
        {
            var estado = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (estado != PermissionStatus.Granted && pedirPermiso)
            {
                estado = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (estado != PermissionStatus.Granted)
            {
                return null;
            }

            var ubicacion = await Geolocation.Default.GetLastKnownLocationAsync()
                ?? await Geolocation.Default.GetLocationAsync(
                    new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(8)));

            if (ubicacion is null)
            {
                return null;
            }

            // Las columnas del backend son DECIMAL(9,6).
            return (Math.Round((decimal)ubicacion.Latitude, 6), Math.Round((decimal)ubicacion.Longitude, 6));
        }
        catch (Exception)
        {
            // GPS apagado, sin soporte en el dispositivo o tiempo agotado: se sigue sin ubicacion.
            return null;
        }
    }
}
