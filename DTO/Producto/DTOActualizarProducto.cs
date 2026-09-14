using System;

namespace DTO.Producto
{
    public class DTOActualizarProducto
    {
        public Guid guidProducto { get; set; }
        public string nombre { get; set; }
        public string marca { get; set; }
        public string descripcion { get; set; }
        public Guid? guidCategoria { get; set; }
        public Guid? guidUnidad { get; set; }
    }
}