using System;

namespace Core.Entidades.Enti_Clases
{
    // Refleja lo que devuelve dbo.SP_OBTENER_MIEMBROS_GUILD.
    public class MiembroGuild
    {
        public Guid guidUsuario { get; set; }
        public string nombre { get; set; }
        public string correo { get; set; }
        public string rolGuild { get; set; }
        public DateTime? ultimoLogin { get; set; }
    }
}
