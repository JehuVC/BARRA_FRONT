using System.Net.Http.Json;

namespace BarraApp.Mobile.Services.Api;

// Base de todo servicio que le pega a la API (SesionService, GuildService, ...).
// Centraliza el manejo de errores de transporte para que cada servicio de
// dominio solo tenga que declarar la ruta y los tipos de DTO/Res.
public abstract class ApiServiceBase(HttpClient http)
{
    protected async Task<TRes> PostAsync<TReq, TRes>(string ruta, TReq body)
    {
        HttpResponseMessage response;
        try
        {
            response = await http.PostAsJsonAsync(ruta, body);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            throw new ApiException("No se pudo conectar con el servidor. Verifique su conexion e intente de nuevo.", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException($"El servidor respondio con un error ({(int)response.StatusCode}).");
        }

        try
        {
            var resultado = await response.Content.ReadFromJsonAsync<TRes>();
            return resultado ?? throw new ApiException("El servidor devolvio una respuesta vacia.");
        }
        catch (System.Text.Json.JsonException ex)
        {
            throw new ApiException("La respuesta del servidor no tiene el formato esperado.", ex);
        }
    }
}
