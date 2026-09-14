using System;

namespace DTO.Comercio
{
    public class DTOActualizarComercio
    {
        public Guid guidComercio { get; set; }
        public string nombre { get; set; }
        public string cadena { get; set; }
        public string direccion { get; set; }
        public decimal? latitud { get; set; }
        public decimal? longitud { get; set; }
        public string telefono { get; set; }
        public string horario { get; set; }
    }
}