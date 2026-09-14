using System;

namespace Core.Entidades.Enti_Clases
{
    // Refleja lo que devuelve dbo.SP_OBTENER_COMERCIOS_CERCANOS.
    public class Comercio
    {
        public Guid guidComercio { get; set; }
        public string nombre { get; set; }
        public string cadena { get; set; }
        public string direccion { get; set; }
        public decimal latitud { get; set; }
        public decimal longitud { get; set; }
        public string telefono { get; set; }
        public string horario { get; set; }
        public double? distanciaKm { get; set; }
    }
}
