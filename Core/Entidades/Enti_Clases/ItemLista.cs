using System;

namespace Core.Entidades.Enti_Clases
{
    // Refleja lo que devuelve dbo.SP_OBTENER_ITEMS_LISTA (join contra TB_PRODUCTO).
    public class ItemLista
    {
        public Guid guidItem { get; set; }
        public Guid guidProducto { get; set; }
        public string nombreProducto { get; set; }
        public string codigoBarras { get; set; }
        public decimal cantidad { get; set; }
        public decimal? precioEstimado { get; set; }
        public bool completado { get; set; }
        public DateTime fechaAgregado { get; set; }
        public DateTime fechaModificacion { get; set; }
    }
}
