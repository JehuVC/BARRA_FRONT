using System;

namespace Core.Entidades.Enti_Clases
{
    // Refleja TB_USUARIO (sin CLAVE_HASH: ese campo nunca debe salir hacia el cliente).
    public class Usuario
    {
        public Guid guid { get; set; }
        public string nombre { get; set; }
        public string correo { get; set; }
        public string zona { get; set; }
        public bool activo { get; set; }
        public Guid? guidGuild { get; set; }
        public string nombreGuild { get; set; }
        public string rolGuild { get; set; }
    }
}
