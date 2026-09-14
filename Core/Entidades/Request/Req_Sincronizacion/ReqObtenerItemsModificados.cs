using System;

namespace Core.Entidades.Request.Req_Sincronizacion
{
    public class ReqObtenerItemsModificados
    {
        public Guid GuidUsuario { get; set; }
        public Guid GuidLista { get; set; }
        public DateTime FechaUltimaSincronizacion { get; set; }
    }
}