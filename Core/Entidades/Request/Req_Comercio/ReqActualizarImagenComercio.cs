using System;

namespace Core.Entidades.Request.Req_Comercio
{
    public class ReqActualizarImagenComercio
    {
        public Guid GuidComercio { get; set; }
        public byte[] ContenidoArchivo { get; set; }
        public string NombreArchivo { get; set; }
        public string ContentType { get; set; }
    }
}
