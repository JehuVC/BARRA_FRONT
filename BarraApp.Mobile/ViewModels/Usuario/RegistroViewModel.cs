using System.Text.RegularExpressions;
using System.Windows.Input;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Sesion;
using DTO.Usuario;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BarraApp.Mobile.ViewModels.Usuario;

public partial class RegistroViewModel : ObservableObject
{
    private readonly ISesionService _sesionService;

    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private string correo = string.Empty;
    [ObservableProperty] private string clave = string.Empty;
    [ObservableProperty] private string confirmarClave = string.Empty;
    [ObservableProperty] private string zona = string.Empty;
    [ObservableProperty] private string mensajeError = string.Empty;
    [ObservableProperty] private bool estaOcupado;

    // Bordes individuales
    [ObservableProperty] private Brush nombreBorder = new SolidColorBrush(Color.FromArgb("#374151"));
    [ObservableProperty] private Brush correoBorder = new SolidColorBrush(Color.FromArgb("#374151"));
    [ObservableProperty] private Brush claveBorder = new SolidColorBrush(Color.FromArgb("#374151"));
    [ObservableProperty] private Brush confirmarClaveBorder = new SolidColorBrush(Color.FromArgb("#374151"));
    [ObservableProperty] private Brush zonaBorder = new SolidColorBrush(Color.FromArgb("#374151"));

    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool PuedeEnviar => !EstaOcupado;

    // Disparadores clave para que la UI se actualice
    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));
    partial void OnEstaOcupadoChanged(bool value) => OnPropertyChanged(nameof(PuedeEnviar));

    public ICommand MostrarInfoCorreoCommand { get; }
    public ICommand MostrarInfoClaveCommand { get; }

    public RegistroViewModel(ISesionService sesionService)
    {
        _sesionService = sesionService;
        MostrarInfoCorreoCommand = new Command(async () => await MostrarInfoCorreoAsync());
        MostrarInfoClaveCommand = new Command(async () => await MostrarInfoClaveAsync());
    }

    private async Task MostrarInfoCorreoAsync()
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Información de Correo",
                "Este correo debe ser real y tener acceso a él, ya que se le enviará un código de verificación.",
                "Entendido");
        }
    }

    private async Task MostrarInfoClaveAsync()
    {
        if (Application.Current?.MainPage != null)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Seguridad de Contraseña",
                "La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula, un número (1) y un carácter especial (*).",
                "Entendido");
        }
    }

    [RelayCommand]
    private async Task RegistrarAsync()
    {
        if (EstaOcupado) return;
        MensajeError = string.Empty;
        RestablecerBordes();

        bool hayError = false;
        var colorRojo = new SolidColorBrush(Color.FromArgb("#EF4444"));

        // 1. Validar Nombre
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            NombreBorder = colorRojo;
            hayError = true;
        }

        // 2. Validar Correo
        if (string.IsNullOrWhiteSpace(Correo))
        {
            CorreoBorder = colorRojo;
            hayError = true;
        }
        else if (!EsEmailValido(Correo))
        {
            CorreoBorder = colorRojo;
            hayError = true;
            if (string.IsNullOrEmpty(MensajeError)) MensajeError = "Ingrese un correo electrónico válido.";
        }

        // 3. Validar Contraseña
        if (string.IsNullOrWhiteSpace(Clave))
        {
            ClaveBorder = colorRojo;
            hayError = true;
        }
        else if (!EsPasswordValida(Clave))
        {
            ClaveBorder = colorRojo;
            hayError = true;
            if (string.IsNullOrEmpty(MensajeError)) MensajeError = "La contraseña no cumple con los requisitos de seguridad.";
        }

        // 4. Validar Confirmar Contraseña
        if (string.IsNullOrWhiteSpace(ConfirmarClave))
        {
            ConfirmarClaveBorder = colorRojo;
            hayError = true;
        }
        else if (Clave != ConfirmarClave)
        {
            ClaveBorder = colorRojo;
            ConfirmarClaveBorder = colorRojo;
            hayError = true;
            if (string.IsNullOrEmpty(MensajeError)) MensajeError = "Las contraseñas no coinciden.";
        }

        // 5. Validar Zona
        if (string.IsNullOrWhiteSpace(Zona))
        {
            ZonaBorder = colorRojo;
            hayError = true;
        }

        // Si hay algún error, detenemos el proceso y mostramos el mensaje final
        if (hayError)
        {
            // Si el mensaje está vacío, significa que el error fue simplemente campos vacíos
            if (string.IsNullOrEmpty(MensajeError))
            {
                MensajeError = "Por favor, completa correctamente todos los campos marcados en rojo.";
            }
            return;
        }

        EstaOcupado = true;
        try
        {
            var dto = new DTORegistro
            {
                nombre = Nombre.Trim(),
                correo = Correo.Trim(),
                pass = Clave,
                zona = Zona.Trim()
            };

            var res = await _sesionService.RegistrarAsync(dto);

            if (!res.resultado)
            {
                MensajeError = res.error?.FirstOrDefault()?.mensaje ?? "Error al registrar la cuenta.";
                return;
            }

            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert("Éxito", "Cuenta creada correctamente. Por favor inicia sesión.", "OK");
            }
            await VolverAsync();
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    [RelayCommand]
    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void RestablecerBordes()
    {
        var grisNormal = new SolidColorBrush(Color.FromArgb("#374151"));
        NombreBorder = grisNormal;
        CorreoBorder = grisNormal;
        ClaveBorder = grisNormal;
        ConfirmarClaveBorder = grisNormal;
        ZonaBorder = grisNormal;
    }

    private bool EsEmailValido(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private bool EsPasswordValida(string pass)
    {
        bool tieneMayuscula = Regex.IsMatch(pass, "[A-Z]");
        bool tieneMinuscula = Regex.IsMatch(pass, "[a-z]");
        bool tieneNumero = Regex.IsMatch(pass, "[0-9]");
        bool tieneEspecial = Regex.IsMatch(pass, "[^a-zA-Z0-9]");

        return pass.Length >= 8 && tieneMayuscula && tieneMinuscula && tieneNumero && tieneEspecial;
    }
}