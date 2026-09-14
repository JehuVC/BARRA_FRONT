using System.Collections.Generic;
using Core.Entidades.Enti_Clases;
using Core.Entidades.Response;

namespace Core.Entidades.Response.Res_Favorito
{
    public class ResObtenerFavoritos : ResBase
    {
        public List<Favorito> favoritos { get; set; }

        public ResObtenerFavoritos()
        {
            favoritos = new List<Favorito>();
        }
    }
}