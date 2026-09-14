using System;

namespace Core.Entidades.Request.Req_Producto
{
    public class ReqRegistrarPrecio
    {
        public Guid GuidProducto { get; set; }
        public Guid GuidComercio { get; set; }
        public decimal Precio { get; set; }
    }
}