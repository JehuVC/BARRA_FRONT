using System;

namespace Core.Entidades.Request.Req_ListaCompra
{
    public class ReqEliminarListaCompra
    {
        public Guid GuidLista { get; set; }
        public Guid GuidUsuario { get; set; }
    }
}