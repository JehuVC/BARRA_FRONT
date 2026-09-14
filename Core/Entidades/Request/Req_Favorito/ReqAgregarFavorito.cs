using System;

namespace Core.Entidades.Request.Req_Favorito
{
    public class ReqAgregarFavorito
    {
        public Guid GuidUsuario { get; set; }
        public Guid GuidProducto { get; set; }
    }
}