using BarraApp.Mobile.Services.Api;
using Core.Entidades.Response;

namespace BarraApp.Mobile.Services.Historial;

public class HistorialService(HttpClient http) : ApiServiceBase(http), IHistorialService
{
    public Task<ResObtenerHistorial> ObtenerHistorialAsync() =>
        // Enviamos un objeto vacío porque el backend usa this.GuidUsuarioActual() desde el JWT
        PostAsync<object, ResObtenerHistorial>("api/historial/obtenerPorUsuario", new { });
}