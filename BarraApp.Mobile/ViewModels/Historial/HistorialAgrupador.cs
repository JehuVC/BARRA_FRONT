using Core.Entidades.Enti_Clases;

namespace BarraApp.Mobile.ViewModels.Historial;

// Arma los bloques del historial a partir de lo que devuelve la API (hasta las ultimas 20 entradas, con la misma
// busqueda repetida en filas distintas): junta las repetidas, deja la mas reciente de cada una y agrupa por dia.
public static class HistorialAgrupador
{
    private class Acumulado
    {
        public string Texto = string.Empty;
        public bool EsProducto;
        public DateTime Fecha;
        public int Veces;
    }

    public static List<GrupoHistorial> Agrupar(IEnumerable<HistorialBusqueda>? entradas, DateTime ahoraUtc)
    {
        var acumulados = new Dictionary<string, Acumulado>();
        var orden = new List<string>();

        // Mas recientes primero: la primera aparicion de cada clave es la mas reciente.
        foreach (var e in (entradas ?? Enumerable.Empty<HistorialBusqueda>()).OrderByDescending(x => x.fecha))
        {
            bool esProducto = e.guidProducto.HasValue && e.guidProducto != Guid.Empty;
            string texto = (esProducto && !string.IsNullOrWhiteSpace(e.nombreProducto)
                ? e.nombreProducto
                : e.terminoBusqueda ?? e.nombreProducto ?? string.Empty).Trim();

            if (texto.Length == 0) continue;

            string clave = esProducto ? "p:" + e.guidProducto : "t:" + texto.ToLowerInvariant();

            if (acumulados.TryGetValue(clave, out var existente))
            {
                existente.Veces++;
            }
            else
            {
                acumulados[clave] = new Acumulado { Texto = texto, EsProducto = esProducto, Fecha = e.fecha, Veces = 1 };
                orden.Add(clave);
            }
        }

        var items = orden
            .Select(c => acumulados[c])
            .Select(a => new HistorialItemViewModel(a.Texto, a.EsProducto, a.Fecha, a.Veces, ahoraUtc))
            .ToList();

        static string Bloque(HistorialItemViewModel i, DateTime ahora)
        {
            int dias = (int)(ahora.Date - i.FechaUtc.Date).TotalDays;
            return dias <= 0 ? "Hoy" : dias == 1 ? "Ayer" : dias < 7 ? "Esta semana" : "Antes";
        }

        string[] titulos = { "Hoy", "Ayer", "Esta semana", "Antes" };
        return titulos
            .Select(t => new GrupoHistorial(t, items.Where(i => Bloque(i, ahoraUtc) == t)))
            .Where(g => g.Count > 0)
            .ToList();
    }
}
