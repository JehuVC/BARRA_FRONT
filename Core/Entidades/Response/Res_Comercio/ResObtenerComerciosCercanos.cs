using System.Collections.Generic;
using Core.Entidades.Enti_Clases;

namespace Core.Entidades.Response
{
    public class ResObtenerComerciosCercanos : ResBase
    {
        public List<Comercio> comercios { get; set; }
    }
}
