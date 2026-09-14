using System;

namespace DTO.Producto
{
    public class DTOBuscarProductos
    {
        public string textoBusqueda { get; set; }
        public Guid? guidCategoria { get; set; }
    }
}