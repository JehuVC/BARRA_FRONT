namespace BarraApp.Mobile.Services.Api;

public static class ApiConfig
{
    // Puerto HTTP de IIS Express para API_4 en el backend (ver la URL que abre
    // Visual Studio al correr API_4 con F5, o Propiedades del proyecto > Depurar).
    // Se usa HTTP y no HTTPS en desarrollo local para no tener que confiar el
    // certificado autofirmado de IIS Express dentro del emulador de Android.
    private const int PuertoApiHttp = 54346;

    public static string BaseUrl => DeviceInfo.Platform == DevicePlatform.Android
        // El emulador de Android tiene su propio "localhost": 10.0.2.2 es el
        // alias que usa para llegar a la PC que lo hospeda.
        ? $"http://10.0.2.2:{PuertoApiHttp}/"
        : $"http://localhost:{PuertoApiHttp}/";
}
