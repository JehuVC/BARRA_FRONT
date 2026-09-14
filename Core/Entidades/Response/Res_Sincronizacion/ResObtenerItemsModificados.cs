using System.Collections.Generic;
using Core.Entidades.Enti_Clases;

namespace Core.Entidades.Response.Res_Sincronizacion
{
    public class ResObtenerItemsModificados : ResBase
    {
        public List<ItemLista> Items { get; set; }
    }
}