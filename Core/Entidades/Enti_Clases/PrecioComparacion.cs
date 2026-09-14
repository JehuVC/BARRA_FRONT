using System;

namespace Core.Entidades.Enti_Clases
{
    public class PrecioComparacion
    {
        public Guid guidPrecio { get; set; }
        public Guid guidComercio { get; set; }
        public string nombreComercio { get; set; }
        public string cadena { get; set; }
        public string direccion { get; set; }
        public decimal precio { get; set; }
        public DateTime fechaActualizacion { get; set; }
        public double? distanciaKm { get; set; }

        // Propiedades calculadas (Punto 18)
        public decimal montoDescuento { get; set; }
        public decimal porcentajeDescuento { get; set; }
    }
}