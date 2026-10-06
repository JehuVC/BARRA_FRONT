using BarraApp.Mobile.Helpers;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Producto;
using BarraApp.Mobile.Views.Producto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enums;

namespace BarraApp.Mobile.ViewModels.Producto;

// Escaner de codigos de barras. Cada codigo leido se busca en el catalogo (api/producto/obtenerPorCodigo):
//   - si existe, se abre su ficha (comparar precios, favorito, agregar a una lista...);
//   - si no existe, se ofrece registrarlo con el codigo ya puesto.
// La camara y el permiso los maneja la pagina (ScannerPage); aqui vive la logica de que hacer con un codigo.
public partial class ScannerViewModel(IProductoService productoService) : ObservableObject
{
    // Si el usuario rechaza registrar un codigo, se ignora ese mismo codigo un rato para que la camara,
    // que sigue apuntando a el, no vuelva a preguntar de inmediato.
    private static readonly TimeSpan TiempoIgnorarRechazado = TimeSpan.FromSeconds(6);

    private string? _codigoIgnorado;
    private DateTime _ignorarHasta;

    [ObservableProperty] private bool camaraActiva;
    [ObservableProperty] private bool pausado;
    [ObservableProperty] private bool linternaEncendida;
    [ObservableProperty] private bool sinPermiso;
    [ObservableProperty] private bool buscando;
    [ObservableProperty] private string mensajeError = string.Empty;

    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool MostrarCamara => !SinPermiso;
    public bool PuedeInteractuar => !Buscando;
    public string Indicacion => Buscando
        ? "Buscando el producto…"
        : "Apunta la cámara al código de barras del producto";

    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));
    partial void OnSinPermisoChanged(bool value) => OnPropertyChanged(nameof(MostrarCamara));

    partial void OnBuscandoChanged(bool value)
    {
        OnPropertyChanged(nameof(PuedeInteractuar));
        OnPropertyChanged(nameof(Indicacion));
    }

    // La pagina avisa si tiene o no permiso de camara; sin permiso se apaga la camara y se muestra la explicacion.
    public void AplicarPermiso(bool concedido)
    {
        SinPermiso = !concedido;
        CamaraActiva = concedido;
        if (concedido) Pausado = false;
    }

    // Cada vez que la pagina vuelve a aparecer (ej. al volver de la ficha de un producto) se puede escanear otro.
    public void Reanudar()
    {
        MensajeError = string.Empty;
        Buscando = false;
        Pausado = false;
    }

    // La pagina deja de verse: se apaga la camara y la linterna.
    public void Detener()
    {
        CamaraActiva = false;
        LinternaEncendida = false;
    }

    // Lo llama la pagina por cada lectura de la camara (ya en el hilo de UI).
    public async Task CodigoLeidoAsync(string? codigoCrudo)
    {
        if (Buscando || Pausado) return;

        string? codigo = ValidacionCliente.NormalizarCodigoBarras(codigoCrudo);
        if (codigo is null) return;

        if (codigo == _codigoIgnorado && DateTime.UtcNow < _ignorarHasta) return;

        await BuscarAsync(codigo);
    }

    [RelayCommand]
    private async Task EscribirCodigoAsync()
    {
        if (Buscando) return;
        Pausado = true;

        try
        {
            string? texto = await Shell.Current.DisplayPromptAsync(
                "Código de barras",
                "Escribe los números que aparecen debajo de las barras.",
                "Buscar",
                "Cancelar",
                placeholder: "Ej. 7441234567890",
                maxLength: 50,
                keyboard: Keyboard.Text);

            if (texto is null) return;

            string? codigo = ValidacionCliente.NormalizarCodigoBarras(texto);
            if (codigo is null)
            {
                MensajeError = "Escribe un código válido, sin espacios (máximo 50 caracteres).";
                return;
            }

            await BuscarAsync(codigo);
        }
        finally
        {
            // Si se navego a otra pantalla, Reanudar() reactiva el escaneo al volver.
            if (!Buscando) Pausado = false;
        }
    }

    [RelayCommand]
    private void AlternarLinterna() => LinternaEncendida = !LinternaEncendida;

    [RelayCommand]
    private void AbrirAjustes() => AppInfo.Current.ShowSettingsUI();

    [RelayCommand]
    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private async Task BuscarAsync(string codigo)
    {
        MensajeError = string.Empty;
        Buscando = true;
        Pausado = true;

        bool seNavego = false;
        try
        {
            var res = await productoService.ObtenerPorCodigoAsync(codigo);

            if (res.resultado && res.producto is not null)
            {
                seNavego = true;
                await Shell.Current.GoToAsync($"{nameof(ProductoDetallePage)}?codigo={Uri.EscapeDataString(codigo)}");
                return;
            }

            if (res.error?.Any(e => e.codigo == enumErrores.productoNoEncontrado) == true)
            {
                bool registrar = await Shell.Current.DisplayAlertAsync(
                    "Producto nuevo",
                    $"El código {codigo} todavía no está en el catálogo. ¿Quieres registrarlo? Así los demás podrán comparar su precio.",
                    "Registrar",
                    "Ahora no");

                if (registrar)
                {
                    seNavego = true;
                    await Shell.Current.GoToAsync($"{nameof(ProductoFormPage)}?codigo={Uri.EscapeDataString(codigo)}");
                }
                else
                {
                    _codigoIgnorado = codigo;
                    _ignorarHasta = DateTime.UtcNow + TiempoIgnorarRechazado;
                }

                return;
            }

            MensajeError = MensajesError.Traducir(res.error, "No se pudo buscar el producto.");
            Ignorar(codigo);
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
            Ignorar(codigo);
        }
        finally
        {
            // Al navegar, la pagina queda pausada hasta que Reanudar() la reactive al volver.
            if (!seNavego)
            {
                Buscando = false;
                Pausado = false;
            }
        }
    }

    private void Ignorar(string codigo)
    {
        _codigoIgnorado = codigo;
        _ignorarHasta = DateTime.UtcNow + TiempoIgnorarRechazado;
    }
}
