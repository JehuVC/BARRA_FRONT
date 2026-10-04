using BarraApp.Mobile.Helpers;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Sesion;
using BarraApp.Mobile.Views.Usuario;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BarraApp.Mobile.ViewModels.Usuario;

// FE-S7-04: pide el correo, el backend envia el codigo y se pasa a RestablecerPage.
public partial class RecuperarViewModel(ISesionService sesionService) : ObservableObject
{
    [ObservableProperty] private string correo = string.Empty;
    [ObservableProperty] private string mensajeError = string.Empty;
    [ObservableProperty] private bool estaOcupado;

    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool PuedeEnviar => !EstaOcupado;

    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));
    partial void OnEstaOcupadoChanged(bool value) => OnPropertyChanged(nameof(PuedeEnviar));

    [RelayCommand]
    private async Task EnviarCodigoAsync()
    {
        if (EstaOcupado) return;
        MensajeError = string.Empty;

        string correoLimpio = Correo.Trim();
        if (!ValidacionCliente.EsEmailValido(correoLimpio))
        {
            MensajeError = "Ingresa un correo electrónico válido.";
            return;
        }

        EstaOcupado = true;
        try
        {
            var res = await sesionService.RecuperarPasswordAsync(correoLimpio);

            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error);
                return;
            }

            // El backend responde igual exista o no el correo (para no revelar cuentas), asi que
            // el mensaje no puede afirmar que el correo se envio.
            await Shell.Current.DisplayAlertAsync(
                "Revisa tu correo",
                "Si el correo está registrado, te enviamos un código de 6 caracteres. Vence en 15 minutos.",
                "Entendido");

            await Shell.Current.GoToAsync($"{nameof(RestablecerPage)}?correo={Uri.EscapeDataString(correoLimpio)}");
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
}
