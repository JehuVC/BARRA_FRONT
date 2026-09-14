using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entidades.Request.Req_Sincronizacion
{
    public class ReqActualizarSincronizacion
    {
        public Guid GuidUsuario { get; set; }
        public bool EsExito { get; set; }
        public string MensajeError { get; set; }
    }
}
