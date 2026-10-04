using Core.Entidades.Response;
using Core.Entidades.Response.Res_Usuario;
using DTO.Usuario;

namespace BarraApp.Mobile.Services.Sesion;

public interface ISesionService
{
    // Ruta api/usuario/login (Método POST)
    Task<DTOResLogin> LoginAsync(string correo, string pass);

    // Ruta api/usuario/registrar (Método POST)
    Task<DTOResRegistrarUsuario> RegistrarAsync(DTORegistro dto);

    // Ruta api/usuario/perfil (Método POST)
    Task<DTOResConsultarPerfil> ObtenerPerfilAsync();

    // Ruta api/usuario/actualizar (Método POST)
    Task<ResActualizarUsuario> ActualizarPerfilAsync(DtoActualizarUsuario dto);

    // Ruta api/usuario/logout (Método POST)
    Task<ResLogout> LogoutAsync();

    // Ruta api/dispositivo/guardarToken (Método POST)
    Task<object> GuardarTokenDispositivoAsync(string tokenDispositivo);

    // Ruta api/usuario/recuperarPassword (Método POST, pública)
    Task<ResRecuperarPassword> RecuperarPasswordAsync(string correo);

    // Ruta api/usuario/validarCodigoRecuperacion (Método POST, pública)
    Task<ResValidarToken> ValidarCodigoRecuperacionAsync(string correo, string codigo);

    // Ruta api/usuario/restablecerClave (Método POST, pública)
    Task<ResRestablecerClave> RestablecerClaveAsync(string correo, string codigo, string nuevaClave);
}