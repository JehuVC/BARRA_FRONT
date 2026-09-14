using System;
using Core.Enums;

namespace Core.Entidades.Enti_Clases
{
    // Refleja los parametros de dbo.SP_INSERTAR_BITACORA.
    public class Bitacora
    {
        public Guid? guidUsuario { get; set; }
        public Guid? guidGuild { get; set; }
        public string clase { get; set; }
        public string metodo { get; set; }
        public enumBitacora tipo { get; set; }
        public int? errorId { get; set; }
        public string descripcion { get; set; }
        public string request { get; set; }
        public string response { get; set; }
    }
}
