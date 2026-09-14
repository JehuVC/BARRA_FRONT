namespace Core.Entidades.Request
{
    public class ReqCompararPrecios
    {
        public string codigoBarras { get; set; }
        public decimal? latitudUsuario { get; set; }
        public decimal? longitudUsuario { get; set; }
    }
}