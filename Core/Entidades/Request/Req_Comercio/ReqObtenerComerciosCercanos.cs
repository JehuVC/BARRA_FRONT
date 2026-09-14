namespace Core.Entidades.Request.Req_Comercio
{
    public class ReqObtenerComerciosCercanos
    {
        public decimal? latitudUsuario { get; set; }
        public decimal? longitudUsuario { get; set; }

        // Opcional: si no viene, Logica usa el mismo default que el SP (5 km).
        public decimal? radioKm { get; set; }
    }
}
