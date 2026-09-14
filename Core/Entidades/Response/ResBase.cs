using System;
using System.Collections.Generic;
using System.Text;
using Core.Entidades.Enti_Clases;
namespace Core.Entidades.Response
{
    public class ResBase
    {
        public bool resultado { get; set; }
        public List<Clase_Error> error { get; set; }
    }
}
