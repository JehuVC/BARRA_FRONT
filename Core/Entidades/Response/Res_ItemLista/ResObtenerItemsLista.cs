using System.Collections.Generic;
using Core.Entidades.Enti_Clases;

namespace Core.Entidades.Response.Res_ItemLista
{
    public class ResObtenerItemsLista : ResBase
    {
        public List<ItemLista> Items { get; set; }

        public ResObtenerItemsLista()
        {
            Items = new List<ItemLista>();
        }
    }
}
