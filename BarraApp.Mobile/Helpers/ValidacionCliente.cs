using System.Text.RegularExpressions;

namespace BarraApp.Mobile.Helpers;

// Validaciones previas al envio, para no gastar una llamada a la API en algo que
// el backend va a rechazar igual. Las reglas del backend (Utilitarios.Validaciones)
// son las que mandan; estas son un poco mas estrictas en la contrasena a proposito.
public static class ValidacionCliente
{
    public const string ReglasPassword =
        "La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial.";

    public static bool EsEmailValido(string? email) =>
        !string.IsNullOrWhiteSpace(email)
        && email.Trim().Length <= 100
        && Regex.IsMatch(email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    public static bool EsPasswordValida(string? pass) =>
        !string.IsNullOrEmpty(pass)
        && pass.Length >= 8
        && Regex.IsMatch(pass, "[A-Z]")
        && Regex.IsMatch(pass, "[a-z]")
        && Regex.IsMatch(pass, "[0-9]")
        && Regex.IsMatch(pass, "[^a-zA-Z0-9]");
}
