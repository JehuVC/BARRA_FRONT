using BarcodeScanning;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Categoria;
using BarraApp.Mobile.Services.Comercio;
using BarraApp.Mobile.Services.Comparador;
using BarraApp.Mobile.Services.Favorito;
using BarraApp.Mobile.Services.Guild;
using BarraApp.Mobile.Services.Historial;
using BarraApp.Mobile.Services.ListaCompra;
using BarraApp.Mobile.Services.Pago;
using BarraApp.Mobile.Services.Producto;
using BarraApp.Mobile.Services.Sesion;
using BarraApp.Mobile.Services.Ubicacion;
using BarraApp.Mobile.Services.Unidad;
using BarraApp.Mobile.ViewModels.Comercio;
using BarraApp.Mobile.ViewModels.Dashboard;
using BarraApp.Mobile.ViewModels.Favorito;
using BarraApp.Mobile.ViewModels.Guild;
using BarraApp.Mobile.ViewModels.Historial;
using BarraApp.Mobile.ViewModels.ListaCompra;
using BarraApp.Mobile.ViewModels.Pago;
using BarraApp.Mobile.ViewModels.Producto;
using BarraApp.Mobile.ViewModels.Usuario;
using BarraApp.Mobile.Views.Comercio;
using BarraApp.Mobile.Views.Dashboard;
using BarraApp.Mobile.Views.Favorito;
using BarraApp.Mobile.Views.Guild;
using BarraApp.Mobile.Views.Historial;
using BarraApp.Mobile.Views.ListaCompra;
using BarraApp.Mobile.Views.Pago;
using BarraApp.Mobile.Views.Producto;
using BarraApp.Mobile.Views.Usuario;
using Microsoft.Extensions.Logging;

namespace BarraApp.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseBarcodeScanning()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        RegistrarApi(builder.Services);
        RegistrarPantallas(builder.Services);

        // Quitar el borde y fondo nativo de los Entry
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoBorder", (handler, view) =>
        {
#if ANDROID
            handler.PlatformView.Background = null;
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif WINDOWS
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            handler.PlatformView.FocusVisualMargin = new Microsoft.UI.Xaml.Thickness(0);
#endif
        });

        return builder.Build();
    }

    static void RegistrarApi(IServiceCollection services)
    {
        services.AddSingleton<IAuthTokenStore, AuthTokenStore>();
        services.AddTransient<AuthHeaderHandler>();

        services.AddHttpClient<ISesionService, SesionService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IHistorialService, HistorialService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IGuildService, GuildService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IListaCompraService, ListaCompraService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IItemListaService, ItemListaService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IProductoService, ProductoService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<ICategoriaService, CategoriaService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IPagoService, PagoService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IUnidadService, UnidadService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IFavoritoService, FavoritoService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IComparadorService, ComparadorService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IComercioService, ComercioService>(client =>
        {
            client.BaseAddress = new Uri(ApiConfig.BaseUrl);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();

        // Servicios locales (no llaman a la API directamente)
        services.AddSingleton<IUbicacionService, UbicacionService>();
        services.AddTransient<IAgregarAListaFlujo, AgregarAListaFlujo>();
    }

    static void RegistrarPantallas(IServiceCollection services)
    {
        services.AddTransient<LoginViewModel>();
        services.AddTransient<LoginPage>();

        services.AddTransient<DashboardViewModel>();
        services.AddTransient<DashboardPage>();

        services.AddTransient<RegistroViewModel>();
        services.AddTransient<RegistroPage>();

        // Registro de Perfil (Nuevo)
        services.AddTransient<PerfilViewModel>();
        services.AddTransient<PerfilPage>();

        // Recuperar contraseña (FE-S7-04 / FE-S7-05)
        services.AddTransient<RecuperarViewModel>();
        services.AddTransient<RecuperarPage>();
        services.AddTransient<RestablecerViewModel>();
        services.AddTransient<RestablecerPage>();

        // Gremio (FE-S7-08 / FE-S7-09 / FE-S7-10)
        services.AddTransient<GuildViewModel>();
        services.AddTransient<GuildPage>();

        // Listas de compra (FE-S9-06..09) y sus productos (FE-S10-01)
        services.AddTransient<ListasViewModel>();
        services.AddTransient<ListasPage>();
        services.AddTransient<ListaDetalleViewModel>();
        services.AddTransient<ListaDetallePage>();

        // Busqueda de productos (favoritos, categorias, agregar a lista)
        services.AddTransient<BusquedaViewModel>();
        services.AddTransient<BusquedaPage>();

        // Detalle de producto (comparar precios, reportar precio)
        services.AddTransient<ProductoDetalleViewModel>();
        services.AddTransient<ProductoDetallePage>();
        services.AddTransient<PagoViewModel>();
        services.AddTransient<PagoPage>();
        services.AddTransient<ScannerViewModel>();
        services.AddTransient<ScannerPage>();
        services.AddTransient<ProductoFormViewModel>();
        services.AddTransient<ProductoFormPage>();

        // Favoritos
        services.AddTransient<FavoritosViewModel>();
        services.AddTransient<FavoritosPage>();

        // Historial completo
        services.AddTransient<HistorialViewModel>();
        services.AddTransient<HistorialPage>();

        // Comercios cercanos y formulario de comercio
        services.AddTransient<ComerciosViewModel>();
        services.AddTransient<ComerciosPage>();
        services.AddTransient<ComercioFormViewModel>();
        services.AddTransient<ComercioFormPage>();
    }
}