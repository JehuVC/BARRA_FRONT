using System;

namespace Core.Entidades.Request.Req_ListaCompra
{
    public class ReqCrearListaCompra
    {
        public Guid GuidLista { get; set; }
        public Guid GuidUsuario { get; set; }
        public Guid? GuidGuild { get; set; } 
        public string Nombre { get; set; }
    }
}