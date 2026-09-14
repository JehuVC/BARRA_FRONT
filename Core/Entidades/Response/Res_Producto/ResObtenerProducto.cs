using Core.Entidades.Enti_Clases;

namespace Core.Entidades.Response
{
    public class ResObtenerProducto : ResBase
    {
        public Producto producto { get; set; }
    }
}
