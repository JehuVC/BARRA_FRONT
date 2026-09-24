using Core.Entidades.Response;

namespace BarraApp.Mobile.Services.Historial;

public interface IHistorialService
{
    Task<ResObtenerHistorial> ObtenerHistorialAsync();
}