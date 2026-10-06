using BarraApp.Mobile.Services.Api;
using Core.Entidades.Response.Res_Favorito;
using DTO.Favorito;

namespace BarraApp.Mobile.Services.Favorito;

public class FavoritoService(HttpClient http) : ApiServiceBase(http), IFavoritoService
{
    public Task<ResObtenerFavoritos> ObtenerAsync() =>
        PostAsync<object, ResObtenerFavoritos>("api/favorito/obtener", new { });

    public Task<ResAgregarFavorito> AgregarAsync(Guid guidProducto) =>
        PostAsync<DTOAgregarFavorito, ResAgregarFavorito>("api/favorito/agregar",
            new DTOAgregarFavorito { GuidProducto = guidProducto });

    public Task<ResQuitarFavorito> QuitarAsync(Guid guidProducto) =>
        PostAsync<DTOQuitarFavorito, ResQuitarFavorito>("api/favorito/quitar",
            new DTOQuitarFavorito { GuidProducto = guidProducto });
}
