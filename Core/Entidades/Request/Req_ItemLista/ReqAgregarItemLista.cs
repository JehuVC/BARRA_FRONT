using System;

namespace Core.Entidades.Request.Req_ItemLista
{
    public class ReqAgregarItemLista
    {
        // Opcional: si el cliente ya genero el GUID offline (sync), lo manda;
        // si no, el SP genera uno nuevo.
        public Guid? GuidItem { get; set; }
        public Guid GuidLista { get; set; }
        public Guid GuidProducto { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? PrecioEstimado { get; set; }
        public Guid GuidUsuario { get; set; }
    }
}
