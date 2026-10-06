using System.Globalization;

namespace BarraApp.Mobile.Helpers;

// Textos para mostrar precios, distancias y fechas, y lectura del precio que escribe el usuario.
public static class Formato
{
    // 1850 -> "₡1,850"   |   1850.5 -> "₡1,850.50"
    public static string Colones(decimal monto) =>
        "₡" + monto.ToString(monto % 1 == 0 ? "#,##0" : "#,##0.00", CultureInfo.InvariantCulture);

    // 2500 CRC -> "₡2,500"   |   9.5 USD -> "$9.50"   |   otra moneda -> "9.50 EUR"
    public static string Monto(decimal monto, string? moneda) => (moneda ?? string.Empty).Trim().ToUpperInvariant() switch
    {
        "CRC" or "" => Colones(monto),
        "USD" => "$" + monto.ToString("#,##0.00", CultureInfo.InvariantCulture),
        var otra => monto.ToString("#,##0.00", CultureInfo.InvariantCulture) + " " + otra
    };

    // 0.35 -> "a 350 m"   |   2.46 -> "a 2.5 km"
    public static string Distancia(double km) =>
        km < 1
            ? $"a {Math.Max(10, (int)(Math.Round(km * 100) * 10))} m"
            : $"a {km.ToString("0.#", CultureInfo.InvariantCulture)} km";

    // "hoy", "ayer", "hace 5 días", o la fecha si pasaron más de un mes. Las fechas del backend vienen en UTC.
    public static string Hace(DateTime fechaUtc, DateTime? ahoraUtc = null)
    {
        int dias = (int)((ahoraUtc ?? DateTime.UtcNow).Date - fechaUtc.Date).TotalDays;
        return dias switch
        {
            <= 0 => "hoy",
            1 => "ayer",
            < 31 => $"hace {dias} días",
            _ => fechaUtc.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
        };
    }

    // Lee una coordenada escrita con punto o coma decimal ("9.9333", "9,9333", "-84.0833") y la redondea a 6 decimales
    // (las columnas del backend son DECIMAL(9,6)). No acepta separador de miles.
    public static bool TryParseCoordenada(string? texto, out decimal valor)
    {
        valor = 0;
        if (string.IsNullOrWhiteSpace(texto)) return false;

        string t = texto.Trim().Replace(',', '.');
        if (t.Count(c => c == '.') > 1) return false;

        if (!decimal.TryParse(t, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out valor))
        {
            return false;
        }

        valor = Math.Round(valor, 6);
        return true;
    }

    // true si dos nombres de comercio son "el mismo": sin importar mayusculas, tildes ni espacios de mas, o si uno
    // contiene al otro ("Pali" / "Pali Zapote"). Se usa para avisar de posibles duplicados al crear un comercio.
    public static bool NombresParecidos(string? a, string? b)
    {
        string x = NormalizarNombre(a), y = NormalizarNombre(b);
        if (x.Length == 0 || y.Length == 0) return false;
        return x == y || (Math.Min(x.Length, y.Length) >= 4 && (x.Contains(y) || y.Contains(x)));
    }

    // true si dos nombres son iguales sin importar mayusculas, tildes ni espacios de mas (sin la regla de "uno contiene
    // al otro"). Se usa para no crear una categoria o unidad que ya existe.
    public static bool NombresIguales(string? a, string? b)
    {
        string x = NormalizarNombre(a);
        return x.Length > 0 && x == NormalizarNombre(b);
    }

    private static string NormalizarNombre(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre)) return string.Empty;

        var sinTildes = new string(nombre.Normalize(System.Text.NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray());

        return string.Join(' ', sinTildes.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    // Lee "1850", "1.850", "1,850", "1850,50" o "₡ 1 850.5". Un solo separador con 1 o 2 dígitos detrás es decimal;
    // con 3 dígitos detrás es separador de miles. Con los dos tipos de separador, el último es el decimal.
    public static bool TryParsePrecio(string? texto, out decimal precio)
    {
        precio = 0;
        if (string.IsNullOrWhiteSpace(texto)) return false;

        string t = new string(texto.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());
        if (t.Length == 0 || !t.Any(char.IsDigit)) return false;

        int ultimoSeparador = Math.Max(t.LastIndexOf('.'), t.LastIndexOf(','));

        if (ultimoSeparador < 0)
        {
            return decimal.TryParse(t, NumberStyles.None, CultureInfo.InvariantCulture, out precio);
        }

        // El mismo separador repetido ("1.234.567" o "1,234,567") son miles: el primer grupo tiene 1 a 3
        // dígitos y todos los demás exactamente 3.
        var separadores = t.Where(c => c == '.' || c == ',').ToList();
        if (separadores.Count > 1 && separadores.Distinct().Count() == 1)
        {
            var grupos = t.Split(separadores[0]);
            if (grupos[0].Length is < 1 or > 3 || grupos.Skip(1).Any(g => g.Length != 3))
            {
                return false;
            }

            return decimal.TryParse(string.Concat(grupos), NumberStyles.None, CultureInfo.InvariantCulture, out precio);
        }

        string enteros = t[..ultimoSeparador].Replace(".", "").Replace(",", "");
        string decimales = t[(ultimoSeparador + 1)..];
        bool unSoloSeparador = t.Count(c => c == '.' || c == ',') == 1;

        if (unSoloSeparador && decimales.Length == 3)
        {
            return decimal.TryParse(enteros + decimales, NumberStyles.None, CultureInfo.InvariantCulture, out precio);
        }

        if (decimales.Length is < 1 or > 2 || enteros.Length == 0)
        {
            return false;
        }

        return decimal.TryParse(enteros + "." + decimales, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out precio);
    }
}
