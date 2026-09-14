using Core.Entidades.Enti_Clases;

namespace Core.Entidades.Response
{
    public class ResLogin : ResBase
    {
        public Usuario usuario { get; set; }
        public Sesion sesion { get; set; }
    }
}
