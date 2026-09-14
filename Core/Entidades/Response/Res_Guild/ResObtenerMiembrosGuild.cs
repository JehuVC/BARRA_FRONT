using Core.Entidades.Enti_Clases;
using System.Collections.Generic;

namespace Core.Entidades.Response
{
    public class ResObtenerMiembrosGuild : ResBase
    {
        public List<MiembroGuild> miembros { get; set; }
    }
}
