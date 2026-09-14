using Core.Entidades.Response;

namespace DTO.Usuario
{
    // Forma publica de ResRegistrarUsuario. Ver DTOResLogin.
    public class DTOResRegistrarUsuario : ResBase
    {
        public DTOUsuarioPublico usuario { get; set; }
    }
}
