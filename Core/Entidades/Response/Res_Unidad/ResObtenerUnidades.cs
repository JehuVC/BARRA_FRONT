using System.Collections.Generic;
using Core.Entidades.Enti_Clases;

namespace Core.Entidades.Response.Res_Unidad
{
    public class ResObtenerUnidades : ResBase
    {
        public List<Unidad> unidades { get; set; }
    }
}