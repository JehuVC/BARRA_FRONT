using System;

namespace Core.Entidades.Request.Req_Usuario
{
    public class ReqActualizarUsuario
    {
        public Guid GuidUsuario { get; set; }
        public string Nombre { get; set; }
        public string Zona { get; set; }
    }
}