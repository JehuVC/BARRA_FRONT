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


	static void RegistrarPantallas(IServiceCollection services)
	{
		services.AddTransient<DashboardViewModel>();
		services.AddTransient<DashboardPage>();
	}
}
