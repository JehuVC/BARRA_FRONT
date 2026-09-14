using Core.Entidades.Enti_Clases;
using System;

namespace Core.Entidades.Request.Req_Guild
{
    public class ReqCrearGuild
    {
        public Guild guild { get; set; }
        public Guid guidUsuarioCreador { get; set; }
    }
}
