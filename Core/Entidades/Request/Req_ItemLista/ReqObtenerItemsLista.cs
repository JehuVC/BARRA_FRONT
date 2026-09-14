using System;

namespace Core.Entidades.Request.Req_ItemLista
{
    public class ReqObtenerItemsLista
    {
        public Guid GuidLista { get; set; }
        public Guid GuidUsuario { get; set; }
    }
}
