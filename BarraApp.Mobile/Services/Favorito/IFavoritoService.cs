using Core.Entidades.Response.Res_Favorito;

namespace BarraApp.Mobile.Services.Favorito;

public interface IFavoritoService
{
    // Ruta api/favorito/obtener (Método POST, el usuario sale del JWT)
    Task<ResObtenerFavoritos> ObtenerAsync();

    // Ruta api/favorito/agregar (Método POST)
    Task<ResAgregarFavorito> AgregarAsync(Guid guidProducto);

    // Ruta api/favorito/quitar (Método POST)
    Task<ResQuitarFavorito> QuitarAsync(Guid guidProducto);
}
