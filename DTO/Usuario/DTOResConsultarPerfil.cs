using System.Collections.Generic;
using Core.Entidades.Enti_Clases;

namespace DTO.Usuario
{
    public class DTOResConsultarPerfil
    {
        public bool resultado { get; set; }
        public List<Clase_Error> error { get; set; }
        public DTOUsuarioPublico usuario { get; set; }
    }
}