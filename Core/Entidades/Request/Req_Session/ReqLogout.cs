using System;

namespace Core.Entidades.Request
{
    public class ReqLogout
    {
        // Cierra TODAS las sesiones activas del usuario (no una sola,
        // porque el cliente nunca tuvo acceso al identificador interno
        // de su token de sesion -- DTOSesion no lo expone a proposito).
        public Guid GuidUsuario { get; set; }
    }
}
