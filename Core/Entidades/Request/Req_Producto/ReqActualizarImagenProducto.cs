using System;

namespace Core.Entidades.Request.Req_Producto
{
    public class ReqActualizarImagenProducto
    {
        public Guid GuidProducto { get; set; }
        public byte[] ContenidoArchivo { get; set; }
        public string NombreArchivo { get; set; }
        public string ContentType { get; set; }
    }
}
