using System;

namespace Core.Entidades.Enti_Clases
{
    public class ListaCompra
    {
        public Guid GuidLista { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public bool Activo { get; set; }
        public Guid? GuidGuild { get; set; }
    }
}