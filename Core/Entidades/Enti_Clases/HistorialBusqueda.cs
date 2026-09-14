using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entidades.Enti_Clases
{
    public class HistorialBusqueda
    {
        public Guid? guidHistorial { get; set; }
        public Guid? guidProducto { get; set; }
        public string terminoBusqueda { get; set; }
        public string nombreProducto { get; set; }
        public DateTime fecha { get; set; }
    }
}