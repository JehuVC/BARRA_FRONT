using System;

namespace DTO.Sesion
{
    // DTO de salida: lo que el cliente necesita de una Sesion tras el login.
    //
    // Sin guidToken (identificador interno de TB_TOKEN) ni estado (bandera
    // interna activa/cerrada) - el cliente se autentica con tokenJwt, no
    // necesita conocer esos datos de implementacion.
    public class DTOSesion
    {
        public string tokenJwt { get; set; }
        public DateTime fechaExpiracion { get; set; }
    }
}
