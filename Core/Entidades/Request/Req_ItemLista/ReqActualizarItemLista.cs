using System;

namespace Core.Entidades.Request.Req_ItemLista
{
    public class ReqActualizarItemLista
    {
        public Guid GuidItem { get; set; }
        public decimal Cantidad { get; set; }
        public decimal? PrecioEstimado { get; set; }
        public bool Completado { get; set; }

        // Deteccion de conflicto last-write-wins, igual que ActualizarLista.
        public DateTime? FechaModificacionCliente { get; set; }
        public Guid GuidUsuario { get; set; }
    }
}
