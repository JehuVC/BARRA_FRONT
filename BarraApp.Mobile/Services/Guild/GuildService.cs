using BarraApp.Mobile.Services.Api;
using Core.Entidades.Response;
using Core.Entidades.Response.Res_Guild;
using DTO.Guild;

namespace BarraApp.Mobile.Services.Guild;

public class GuildService(HttpClient http) : ApiServiceBase(http), IGuildService
{
    public Task<ResCrearGuild> CrearAsync(string nombre, string? descripcion) =>
        PostAsync<DTOCrearGuild, ResCrearGuild>("api/guild/crear",
            new DTOCrearGuild { nombre = nombre, descripcion = descripcion ?? string.Empty });

    public Task<ResUnirseGuild> UnirseAsync(string codigoInvitacion) =>
        PostAsync<DTOUnirseGuild, ResUnirseGuild>("api/guild/unirse",
            new DTOUnirseGuild { CodigoInvitacion = codigoInvitacion });

    public Task<ResObtenerMiembrosGuild> ObtenerMiembrosAsync() =>
        PostAsync<object, ResObtenerMiembrosGuild>("api/guild/obtenerMiembros", new { });

    public Task<ResSalirGuild> SalirAsync() =>
        PostAsync<object, ResSalirGuild>("api/guild/salir", new { });
}
