using System;

namespace Core.Entidades.Request.Req_Sincronizacion
{
    public class ReqObtenerListasModificadas
    {
        public Guid GuidUsuario { get; set; }
        public DateTime FechaUltimaSincronizacion { get; set; }
    }
}