using BarraApp.Mobile.Services.Api;
using Core.Entidades.Response;
using Core.Entidades.Response.Res_Usuario;
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

    // Ruta api/usuario/perfil (Método POST)
    public Task<DTOResConsultarPerfil> ObtenerPerfilAsync() =>
        PostAsync<object, DTOResConsultarPerfil>("api/usuario/perfil", new { });

    // Ruta api/usuario/actualizar (Método POST)
    public Task<ResActualizarUsuario> ActualizarPerfilAsync(DtoActualizarUsuario dto) =>
        PostAsync<DtoActualizarUsuario, ResActualizarUsuario>("api/usuario/actualizar", dto);

    // Ruta api/usuario/logout (Método POST)
    public Task<ResLogout> LogoutAsync() =>
        PostAsync<object, ResLogout>("api/usuario/logout", new { });

    // Ruta api/dispositivo/guardarToken (Método POST)
    public Task<object> GuardarTokenDispositivoAsync(string tokenDispositivo) =>
        PostAsync<object, object>("api/dispositivo/guardarToken", new { tokenDispositivo });
}