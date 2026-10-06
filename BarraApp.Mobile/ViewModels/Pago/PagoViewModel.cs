using BarraApp.Mobile.Helpers;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Pago;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entidades.Enti_Clases;

namespace BarraApp.Mobile.ViewModels.Pago;

// Pagos con Tilopay. Tres estados en una misma pantalla:
//   1. Planes: se elige un plan y se crea la orden (api/pago/crear), que devuelve la URL de checkout.
//   2. Checkout: la URL se abre en un WebView. Tilopay no avisa a la app cuando terminas, asi que mientras el
//      WebView esta abierto se pregunta cada pocos segundos por el estado de la orden (api/pago/consultar).
//   3. Resultado: aprobado, rechazado o en proceso.
// El guid de la orden se guarda en Preferences para poder reanudar o verificar un pago que se dejo a medias.
public partial class PagoViewModel(IPagoService pagoService) : ObservableObject
{
    private const string ClaveOrdenPendiente = "pago_orden_pendiente";
    private static readonly TimeSpan IntervaloVerificacion = TimeSpan.FromSeconds(4);
    private static readonly TimeSpan DuracionMaximaVerificacion = TimeSpan.FromMinutes(15);

    private CancellationTokenSource? _vigilancia;
    private Guid _guidOrden;
    private string? _urlPendiente;
    private bool _consultando;

    [ObservableProperty] private bool mostrarCheckout;
    [ObservableProperty] private bool mostrarResultado;
    [ObservableProperty] private bool procesando;
    [ObservableProperty] private bool hayPendiente;
    [ObservableProperty] private string? urlCheckout;
    [ObservableProperty] private string mensajeError = string.Empty;
    [ObservableProperty] private string avisoCheckout = string.Empty;

    [ObservableProperty] private string resultadoIcono = string.Empty;
    [ObservableProperty] private string resultadoTitulo = string.Empty;
    [ObservableProperty] private string resultadoMensaje = string.Empty;
    [ObservableProperty] private string resultadoDetalle = string.Empty;
    [ObservableProperty] private bool resultadoExitoso;

    public IReadOnlyList<PlanPago> Planes => PlanesPago.Todos;
    public bool EsModoPruebas => PlanesPago.ModoPruebas;
    public bool MostrarPlanes => !MostrarCheckout && !MostrarResultado;
    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool HayAvisoCheckout => !string.IsNullOrWhiteSpace(AvisoCheckout);
    public bool PuedeInteractuar => !Procesando;

    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));
    partial void OnAvisoCheckoutChanged(string value) => OnPropertyChanged(nameof(HayAvisoCheckout));
    partial void OnProcesandoChanged(bool value) => OnPropertyChanged(nameof(PuedeInteractuar));
    partial void OnMostrarCheckoutChanged(bool value) => OnPropertyChanged(nameof(MostrarPlanes));
    partial void OnMostrarResultadoChanged(bool value) => OnPropertyChanged(nameof(MostrarPlanes));

    // Se corre cada vez que aparece la pantalla: si quedo una orden a medias, se revisa en que estado esta.
    [RelayCommand]
    public async Task CargarAsync()
    {
        if (MostrarCheckout)
        {
            // Se volvio a la pantalla con el checkout abierto: se retoma la verificacion si se habia detenido.
            if (_vigilancia is null)
            {
                _vigilancia = new CancellationTokenSource();
                _ = VigilarAsync(_vigilancia.Token);
            }

            return;
        }

        if (MostrarResultado || Procesando) return;

        string guardado = Preferences.Default.Get(ClaveOrdenPendiente, string.Empty);
        if (!Guid.TryParse(guardado, out Guid guid) || guid == Guid.Empty)
        {
            HayPendiente = false;
            return;
        }

        _guidOrden = guid;
        var orden = await ConsultarAsync(silencioso: true);

        if (orden is null)
        {
            // Sin conexion: se deja el aviso para reintentar mas tarde.
            HayPendiente = true;
            return;
        }

        if (PagoEstado.EsFinal(orden.estado))
        {
            LimpiarOrdenGuardada();
            MostrarResultadoDe(orden);
            return;
        }

        _urlPendiente = orden.urlCheckout;
        HayPendiente = true;
    }

    [RelayCommand]
    private async Task ElegirPlanAsync(PlanPago plan)
    {
        if (plan is null || Procesando) return;
        MensajeError = string.Empty;

        bool continuar = await Shell.Current.DisplayAlertAsync(
            plan.Nombre,
            $"Vas a pagar {plan.PrecioTexto} con tarjeta. El pago se completa en la ventana segura de Tilopay.",
            "Continuar",
            "Cancelar");

        if (!continuar) return;

        Procesando = true;
        try
        {
            var res = await pagoService.CrearAsync(plan.Monto, plan.Moneda);
            if (!res.resultado || string.IsNullOrWhiteSpace(res.UrlCheckout))
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudo iniciar el pago.");
                return;
            }

            _guidOrden = res.GuidOrdenPago;
            Preferences.Default.Set(ClaveOrdenPendiente, _guidOrden.ToString());
            HayPendiente = false;
            AbrirCheckout(res.UrlCheckout);
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            Procesando = false;
        }
    }

    // El usuario dejo un pago a medias: vuelve a abrir el mismo checkout.
    [RelayCommand]
    private void ContinuarPendiente()
    {
        if (string.IsNullOrWhiteSpace(_urlPendiente))
        {
            MensajeError = "No pudimos recuperar el pago. Verifica su estado o inicia uno nuevo.";
            return;
        }

        AbrirCheckout(_urlPendiente);
    }

    // Boton "Ya pagué": una consulta manual, por si el usuario no quiere esperar a la verificacion automatica.
    [RelayCommand]
    private async Task VerificarAsync()
    {
        if (_guidOrden == Guid.Empty || Procesando) return;
        MensajeError = string.Empty;
        AvisoCheckout = string.Empty;

        Procesando = true;
        try
        {
            var orden = await ConsultarAsync(silencioso: false);
            if (orden is null) return;

            if (PagoEstado.EsFinal(orden.estado))
            {
                DetenerVigilancia();
                LimpiarOrdenGuardada();
                MostrarResultadoDe(orden);
            }
            else if (MostrarCheckout)
            {
                AvisoCheckout = "Todavía no vemos el pago. Termina de pagar en la ventana y vuelve a tocar «Ya pagué».";
            }
            else
            {
                MensajeError = "El pago sigue en proceso. Intenta de nuevo en unos segundos.";
            }
        }
        finally
        {
            Procesando = false;
        }
    }

    [RelayCommand]
    private async Task AbrirEnNavegadorAsync()
    {
        if (string.IsNullOrWhiteSpace(UrlCheckout)) return;

        try
        {
            await Browser.Default.OpenAsync(UrlCheckout, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception)
        {
            MensajeError = "No pudimos abrir el navegador.";
        }
    }

    // Sale del checkout sin terminar. La orden queda guardada para poder verificarla despues.
    [RelayCommand]
    private async Task SalirDelCheckoutAsync()
    {
        bool salir = await Shell.Current.DisplayAlertAsync(
            "¿Salir del pago?",
            "Si ya pagaste, puedes verificar el resultado después desde esta misma pantalla.",
            "Salir",
            "Seguir pagando");

        if (!salir) return;

        DetenerVigilancia();
        CerrarCheckout();
        await CargarAsync();
    }

    [RelayCommand]
    private void NuevoIntento()
    {
        MostrarResultado = false;
        MensajeError = string.Empty;
    }

    [RelayCommand]
    private async Task VolverAsync()
    {
        if (MostrarCheckout)
        {
            await SalirDelCheckoutAsync();
            return;
        }

        if (MostrarResultado)
        {
            MostrarResultado = false;
            return;
        }

        await Shell.Current.GoToAsync("..");
    }

    // La pantalla se cierra (OnDisappearing): se deja de preguntar a Tilopay.
    public void Detener() => DetenerVigilancia();

    private void AbrirCheckout(string url)
    {
        MensajeError = string.Empty;
        AvisoCheckout = string.Empty;
        UrlCheckout = url;
        MostrarCheckout = true;

        DetenerVigilancia();
        _vigilancia = new CancellationTokenSource();
        _ = VigilarAsync(_vigilancia.Token);
    }

    private void CerrarCheckout()
    {
        MostrarCheckout = false;
        UrlCheckout = null;
        AvisoCheckout = string.Empty;
    }

    // Mientras el checkout esta abierto, pregunta por el estado hasta que sea final, se cierre la pantalla o pasen
    // 15 minutos. Los errores de red se ignoran: el siguiente intento lo resuelve.
    private async Task VigilarAsync(CancellationToken ct)
    {
        DateTime inicio = DateTime.UtcNow;

        try
        {
            while (!ct.IsCancellationRequested && DateTime.UtcNow - inicio < DuracionMaximaVerificacion)
            {
                await Task.Delay(IntervaloVerificacion, ct);

                var orden = await ConsultarAsync(silencioso: true);
                if (ct.IsCancellationRequested) return;

                if (orden is not null && PagoEstado.EsFinal(orden.estado))
                {
                    LimpiarOrdenGuardada();
                    MostrarResultadoDe(orden);
                    return;
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    // Una consulta a la vez. silencioso = no muestra errores (verificacion automatica o al abrir la pantalla).
    private async Task<OrdenPago?> ConsultarAsync(bool silencioso)
    {
        if (_guidOrden == Guid.Empty || _consultando) return null;

        _consultando = true;
        try
        {
            var res = await pagoService.ConsultarAsync(_guidOrden);
            if (!res.resultado || res.Orden is null)
            {
                if (!silencioso) MensajeError = MensajesError.Traducir(res.error, "No se pudo consultar el pago.");
                else if (res.error?.Any(e => e.codigo == Core.Enums.enumErrores.ordenPagoNoEncontrada) == true)
                {
                    // La orden guardada ya no existe (ej. base de datos reiniciada): se olvida.
                    LimpiarOrdenGuardada();
                }

                return null;
            }

            return res.Orden;
        }
        catch (ApiException ex)
        {
            if (!silencioso) MensajeError = ex.Message;
            return null;
        }
        finally
        {
            _consultando = false;
        }
    }

    private void MostrarResultadoDe(OrdenPago orden)
    {
        var r = PagoEstado.Resolver(orden);
        ResultadoIcono = r.Icono;
        ResultadoTitulo = r.Titulo;
        ResultadoMensaje = r.Mensaje;
        ResultadoDetalle = r.Detalle;
        ResultadoExitoso = r.Exitoso;

        DetenerVigilancia();
        CerrarCheckout();
        HayPendiente = false;
        MostrarResultado = true;
    }

    private void LimpiarOrdenGuardada()
    {
        Preferences.Default.Remove(ClaveOrdenPendiente);
        _guidOrden = Guid.Empty;
        _urlPendiente = null;
        HayPendiente = false;
    }

    private void DetenerVigilancia()
    {
        _vigilancia?.Cancel();
        _vigilancia?.Dispose();
        _vigilancia = null;
    }
}
