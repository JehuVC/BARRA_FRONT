using System;

namespace DTO.Usuario
{
    // DTO de salida: la forma publica de Usuario que ve el cliente.
    //
    // A proposito sin "activo" (bandera interna de borrado logico) -
    // mismo criterio por el que la entidad Usuario de Core ya excluye
    // CLAVE_HASH: si mañana Usuario gana un campo interno nuevo, no se
    // filtra solo porque nadie se acuerde de excluirlo aqui.
    public class DTOUsuarioPublico
    {
        public Guid guid { get; set; }
        public string nombre { get; set; }
        public string correo { get; set; }
        public string zona { get; set; }
        public Guid? guidGuild { get; set; }
        public string nombreGuild { get; set; }
        public string rolGuild { get; set; }
    }
}
