using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Sesion;
using BarraApp.Mobile.ViewModels.Dashboard;
using BarraApp.Mobile.Views.Dashboard;
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

		return builder.Build();
	}

	// Cliente HTTP tipado por servicio de dominio (FE-S6-02): cada servicio
	// nuevo que consuma la API se agrega aqui igual que ISesionService/SesionService.
	static void RegistrarApi(IServiceCollection services)
	{
		services.AddSingleton<IAuthTokenStore, AuthTokenStore>();
		services.AddTransient<AuthHeaderHandler>();

		services.AddHttpClient<ISesionService, SesionService>(client =>
		{
			client.BaseAddress = new Uri(ApiConfig.BaseUrl);
		})
		.AddHttpMessageHandler<AuthHeaderHandler>();
	}

	// Cada pantalla nueva (Views/<Dominio>/XxxPage + ViewModels/<Dominio>/XxxViewModel)
	// se registra aqui como Transient para que Shell pueda inyectar el ViewModel
	// por constructor al navegar a su ruta.
	static void RegistrarPantallas(IServiceCollection services)
	{
		services.AddTransient<DashboardViewModel>();
		services.AddTransient<DashboardPage>();
	}
}
