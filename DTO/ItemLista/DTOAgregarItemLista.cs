using System;

namespace DTO.ItemLista
{
    public class DTOAgregarItemLista
    {
        public Guid? GuidItem { get; set; }
        public Guid GuidLista { get; set; }
        public Guid GuidProducto { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? PrecioEstimado { get; set; }
    }
}
