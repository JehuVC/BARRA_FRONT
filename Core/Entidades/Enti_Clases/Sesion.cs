using System;
using Core.Enums;

namespace Core.Entidades.Enti_Clases
{
    // Refleja lo que el cliente necesita de TB_TOKEN tras un login exitoso.
    public class Sesion
    {
        public Guid guidToken { get; set; }
        public string tokenJwt { get; set; }
        public DateTime fechaExpiracion { get; set; }
        public enumEstadoSesion estado { get; set; }
    }
}
