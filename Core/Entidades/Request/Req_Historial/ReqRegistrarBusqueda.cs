using System;

namespace Core.Entidades.Request.Req_Historial
{
    public class ReqRegistrarBusqueda
    {
        public Guid GuidUsuario { get; set; }
        public string TerminoBusqueda { get; set; }
        public Guid? GuidProducto { get; set; }
    }
}