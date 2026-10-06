using Core.Entidades.Response;
using DTO.Comercio;

namespace BarraApp.Mobile.Services.Comercio;

public interface IComercioService
{
    // Ruta api/comercio/obtenerCercanos (Método POST). Ordenados del mas cercano al mas lejano.
    Task<ResObtenerComerciosCercanos> ObtenerCercanosAsync(decimal latitud, decimal longitud, decimal radioKm);

    // Ruta api/comercio/crear (Método POST). El backend no revisa duplicados: eso lo hace la pantalla.
    Task<ResCrearComercio> CrearAsync(DTOCrearComercio dto);

    // Ruta api/comercio/actualizar (Método POST). Reemplaza todos los datos del comercio.
    Task<ResActualizarComercio> ActualizarAsync(DTOActualizarComercio dto);
}
