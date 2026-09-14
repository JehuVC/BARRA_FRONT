using System.Collections.Generic;
using Core.Entidades.Enti_Clases;

namespace Core.Entidades.Response.Res_Producto
{
    public class ResBuscarProductos : ResBase
    {
        public List<Producto> productos { get; set; }
    }
}