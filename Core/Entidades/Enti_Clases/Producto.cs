using System;

namespace Core.Entidades.Enti_Clases
{
    // Refleja las columnas reales que devuelve dbo.SP_OBTENER_PRODUCTO_POR_CODIGO_BARRAS.
    public class Producto
    {
        public Guid guidProducto { get; set; }
        public string codigoBarras { get; set; }
        public string nombre { get; set; }
        public string marca { get; set; }
        public string descripcion { get; set; }
        public Guid? guidCategoria { get; set; }
        public string nombreCategoria { get; set; }
        public Guid? guidUnidad { get; set; }
        public string nombreUnidad { get; set; }
    }
}
