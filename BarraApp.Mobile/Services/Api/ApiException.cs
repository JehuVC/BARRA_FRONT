namespace BarraApp.Mobile.Services.Api;

// Fallo de transporte (sin conexion, timeout, 500, JSON invalido) -- distinto
// de un ResBase con resultado=false, que es un fallo de negocio normal y
// esperado (ej. credenciales invalidas) que cada pantalla ya sabe leer del
// campo "error".
public class ApiException(string message, Exception? innerException = null)
    : Exception(message, innerException);
