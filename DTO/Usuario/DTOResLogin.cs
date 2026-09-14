using Core.Entidades.Response;
using DTO.Sesion;

namespace DTO.Usuario
{
    // Forma publica de ResLogin: reemplaza las entidades Usuario y Sesion
    // completas por sus DTO de salida. La arma AdapUsuario.ToDto(ResLogin),
    // no Logica -- Logica sigue devolviendo el ResLogin de Core (con las
    // entidades reales) sin tocar.
    public class DTOResLogin : ResBase
    {
        public DTOUsuarioPublico usuario { get; set; }
        public DTOSesion sesion { get; set; }
    }
}
