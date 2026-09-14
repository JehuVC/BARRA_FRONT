using System;

namespace DTO.ItemLista
{
    public class DTOActualizarItemLista
    {
        public Guid GuidItem { get; set; }
        public decimal Cantidad { get; set; }
        public decimal? PrecioEstimado { get; set; }
        public bool Completado { get; set; }
        public DateTime? FechaModificacionCliente { get; set; }
    }
}
