using System;

namespace DTO.ItemLista
{
    public class DTOObtenerItemsModificados
    {
        public Guid GuidLista { get; set; }
        public DateTime FechaUltimaSincronizacion { get; set; }
    }
}