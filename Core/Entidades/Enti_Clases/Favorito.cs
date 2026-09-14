using System;

namespace Core.Entidades.Enti_Clases
{
    public class Favorito
    {
        public Guid guidProducto { get; set; }
        public string nombreProducto { get; set; }
        public string codigoBarras { get; set; }
        public string marca { get; set; }
        public DateTime? fechaAgregado { get; set; }
    }
}