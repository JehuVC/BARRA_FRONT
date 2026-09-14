using System;

namespace Core.Entidades.Request.Req_ItemLista
{
    public class ReqEliminarItemLista
    {
        public Guid GuidItem { get; set; }
        public Guid GuidUsuario { get; set; }
    }
}
