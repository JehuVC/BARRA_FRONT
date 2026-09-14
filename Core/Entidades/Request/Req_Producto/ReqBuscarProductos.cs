using System;

namespace Core.Entidades.Request.Req_Producto
{
    public class ReqBuscarProductos
    {
        public string textoBusqueda { get; set; }
        public Guid? guidCategoria { get; set; }
    }
}