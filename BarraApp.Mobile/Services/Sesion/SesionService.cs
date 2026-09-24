using BarraApp.Mobile.Services.Api;
using DTO.Usuario;

namespace BarraApp.Mobile.Services.Sesion;

public class SesionService(HttpClient http) : ApiServiceBase(http), ISesionService
{
    // Ruta api/usuario/login (Método POST)
    public Task<DTOResLogin> LoginAsync(string correo, string pass) =>
        PostAsync<DTOLogin, DTOResLogin>("api/usuario/login", new DTOLogin { correo = correo, pass = pass });

    // Ruta api/usuario/registrar (Método POST)
    public Task<DTOResRegistrarUsuario> RegistrarAsync(DTORegistro dto) =>
        PostAsync<DTORegistro, DTOResRegistrarUsuario>("api/usuario/registrar", dto);


}