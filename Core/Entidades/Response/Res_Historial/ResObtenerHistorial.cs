using System.Collections.Generic;
using Core.Entidades.Enti_Clases;

namespace Core.Entidades.Response
{
    public class ResObtenerHistorial : ResBase
    {
        public List<HistorialBusqueda> historial { get; set; }
    }
}
