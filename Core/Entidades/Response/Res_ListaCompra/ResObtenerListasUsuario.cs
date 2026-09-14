using System.Collections.Generic;
using Core.Entidades.Enti_Clases; 

namespace Core.Entidades.Response.Res_ListaCompra
{
    public class ResObtenerListasUsuario : ResBase
    {
        public List<ListaCompra> Listas { get; set; }

        public ResObtenerListasUsuario()
        {
            Listas = new List<ListaCompra>();
        }
    }
}