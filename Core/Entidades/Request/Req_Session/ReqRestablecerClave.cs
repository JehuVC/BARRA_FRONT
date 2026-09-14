namespace Core.Entidades.Request
{
    public class ReqRestablecerClave
    {
        public string correo { get; set; }
        public string codigo { get; set; }
        public string nuevaClave { get; set; }
    }
}
