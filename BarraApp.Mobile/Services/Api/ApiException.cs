namespace BarraApp.Mobile.Services.Api;


public class ApiException(string message, Exception? innerException = null)
    : Exception(message, innerException);
