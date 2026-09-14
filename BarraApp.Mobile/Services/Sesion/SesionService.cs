using BarraApp.Mobile.Services.Api;
using DTO.Usuario;

namespace BarraApp.Mobile.Services.Sesion;

public class SesionService(HttpClient http) : ApiServiceBase(http), ISesionService
{
    public Task<DTOResLogin> LoginAsync(string correo, string pass) =>
        PostAsync<DTOLogin, DTOResLogin>("api/usuario/login", new DTOLogin { correo = correo, pass = pass });
}
