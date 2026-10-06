using BarraApp.Mobile.Services.Api;
using Core.Entidades.Response;
using Core.Entidades.Response.Res_Historial;
using DTO.Historial;

namespace BarraApp.Mobile.Services.Historial;

public class HistorialService(HttpClient http) : ApiServiceBase(http), IHistorialService
{
    public Task<ResObtenerHistorial> ObtenerHistorialAsync() =>
        // Enviamos un objeto vacío porque el backend usa this.GuidUsuarioActual() desde el JWT
        PostAsync<object, ResObtenerHistorial>("api/historial/obtenerPorUsuario", new { });

    public Task<ResRegistrarBusqueda> RegistrarBusquedaAsync(string? terminoBusqueda, Guid? guidProducto = null) =>
        PostAsync<DTORegistrarBusqueda, ResRegistrarBusqueda>("api/historial/registrar",
            new DTORegistrarBusqueda { TerminoBusqueda = terminoBusqueda ?? string.Empty, GuidProducto = guidProducto });
}
