using System;

namespace DTO.ListaCompra
{
    public class DTOActualizarListaCompra
    {
        public Guid GuidLista { get; set; }
        public string Nombre { get; set; }
        public DateTime? FechaModificacionCliente { get; set; }
    }
}
