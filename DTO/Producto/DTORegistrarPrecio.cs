using System;

namespace DTO.Producto
{
    public class DTORegistrarPrecio
    {
        public Guid GuidProducto { get; set; }
        public Guid GuidComercio { get; set; }
        public decimal Precio { get; set; }
    }
}