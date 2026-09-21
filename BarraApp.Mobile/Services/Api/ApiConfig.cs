namespace BarraApp.Mobile.Services.Api;

public static class ApiConfig
{
 
    private const int PuertoApiHttp = 54346;

    public static string BaseUrl => DeviceInfo.Platform == DevicePlatform.Android
  
        ? $"http://10.0.2.2:{PuertoApiHttp}/"
        : $"http://localhost:{PuertoApiHttp}/";
}
