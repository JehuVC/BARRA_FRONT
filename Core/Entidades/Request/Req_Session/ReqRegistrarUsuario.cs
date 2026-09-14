using Core.Entidades.Enti_Clases;

namespace Core.Entidades.Request
{
    public class ReqRegistrarUsuario
    {
        public Usuario usuario { get; set; }

        public string password { get; set; }
    }
}
