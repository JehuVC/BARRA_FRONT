using DTO.Usuario;

namespace BarraApp.Mobile.Services.Sesion;

public interface ISesionService
{
    Task<DTOResLogin> LoginAsync(string correo, string pass);
    Task<DTOResRegistrarUsuario> RegistrarAsync(DTORegistro dto);

}