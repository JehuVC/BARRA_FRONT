using Core.Entidades.Enti_Clases;

namespace Core.Entidades.Response.Res_Producto
{
    public class ResActualizarProducto : ResBase
    {
        public Producto producto { get; set; }
    }
}