using System.Collections.Generic;
using Core.Entidades.Enti_Clases;

namespace Core.Entidades.Response.Res_Usuario
{
    public class ResConsultarPerfil
    {
        public bool resultado { get; set; }
        public List<Clase_Error> error { get; set; }
        public Usuario usuario { get; set; }
    }
}