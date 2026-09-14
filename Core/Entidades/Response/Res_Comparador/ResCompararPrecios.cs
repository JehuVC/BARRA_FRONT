using System.Collections.Generic;
using Core.Entidades.Enti_Clases;

namespace Core.Entidades.Response
{
    public class ResCompararPrecios : ResBase
    {
        public List<PrecioComparacion> precios { get; set; }
    }
}