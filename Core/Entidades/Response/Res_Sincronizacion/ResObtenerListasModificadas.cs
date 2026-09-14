using System;
using System.Collections.Generic;
using System.Collections.Generic;
using Core.Entidades.Enti_Clases;

namespace Core.Entidades.Response.Res_Sincronizacion
{
    public class ResObtenerListasModificadas : ResBase
    {
       
        public List<ListaCompra> Listas { get; set; }
    }
}