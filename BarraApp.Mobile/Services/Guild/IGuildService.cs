using Core.Entidades.Response;
using Core.Entidades.Response.Res_Guild;

namespace BarraApp.Mobile.Services.Guild;

public interface IGuildService
{
    // Ruta api/guild/crear (Método POST)
    Task<ResCrearGuild> CrearAsync(string nombre, string? descripcion);

    // Ruta api/guild/unirse (Método POST)
    Task<ResUnirseGuild> UnirseAsync(string codigoInvitacion);

    // Ruta api/guild/obtenerMiembros (Método POST, el guild sale del JWT)
    Task<ResObtenerMiembrosGuild> ObtenerMiembrosAsync();

    // Ruta api/guild/salir (Método POST, el guild sale del JWT)
    Task<ResSalirGuild> SalirAsync();
}
