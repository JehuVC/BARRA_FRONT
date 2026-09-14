using System;

namespace Core.Entidades.Request.Req_Guild
{
    public class ReqUnirseGuild
    {
        public Guid GuidUsuario { get; set; }
        public string CodigoInvitacion { get; set; }
    }
}