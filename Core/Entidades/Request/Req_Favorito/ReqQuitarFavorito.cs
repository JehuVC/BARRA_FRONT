using System;

namespace Core.Entidades.Request.Req_Favorito
{
    public class ReqQuitarFavorito
    {
        public Guid GuidUsuario { get; set; }
        public Guid GuidProducto { get; set; }
    }
}