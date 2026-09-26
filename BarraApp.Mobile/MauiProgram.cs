using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Historial;
using BarraApp.Mobile.Services.Sesion;
using BarraApp.Mobile.ViewModels.Dashboard;
using BarraApp.Mobile.ViewModels.Usuario;
using BarraApp.Mobile.Views.Dashboard;
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
    }
}