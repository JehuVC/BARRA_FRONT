using System;

namespace Core.Entidades.Request.Req_ListaCompra
{
    public class ReqActualizarListaCompra
    {
        public Guid GuidLista { get; set; }
        public Guid GuidUsuario { get; set; }
        public string Nombre { get; set; }
        public DateTime? FechaModificacionCliente { get; set; }
    }
}