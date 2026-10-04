namespace BarraApp.Mobile.Services.Api;

public static class ApiConfig
{
    // Puerto HTTP de IIS Express para API_4 (el mismo de Barra_AP-VF/API_4/API_4.csproj).
    private const int PuertoApiHttp = 54346;

    // "localhost" en todas las plataformas. El IIS Express de Visual Studio solo atiende el host
    // "localhost": si el emulador llamara a 10.0.2.2 recibiria HTTP 400 "Invalid Hostname".
    // En Android, "localhost" es el propio telefono, asi que antes de probar hay que reenviar el
    // puerto a la PC con scripts/iniciar_dev_android.ps1 (adb reverse tcp:54346 tcp:54346).
    public static string BaseUrl => $"http://localhost:{PuertoApiHttp}/";
}
