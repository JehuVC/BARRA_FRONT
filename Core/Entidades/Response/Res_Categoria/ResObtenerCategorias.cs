using System.Collections.Generic;
using Core.Entidades.Enti_Clases;

namespace Core.Entidades.Response
{
    public class ResObtenerCategorias : ResBase
    {
        public List<Categoria> categorias { get; set; }
    }
}