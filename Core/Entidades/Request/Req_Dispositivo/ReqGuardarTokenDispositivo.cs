using System;

namespace Core.Entidades.Request.Req_Dispositivo
{
    public class ReqGuardarTokenDispositivo
    {
        public Guid GuidUsuario { get; set; }
        public string TokenDispositivo { get; set; }
        public string Plataforma { get; set; }
    }
}
