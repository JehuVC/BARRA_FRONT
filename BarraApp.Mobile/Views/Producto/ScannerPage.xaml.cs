using BarcodeScanning;
using BarraApp.Mobile.ViewModels.Producto;

namespace BarraApp.Mobile.Views.Producto;

public partial class ScannerPage : ContentPage
{
    private readonly ScannerViewModel _viewModel;

    public ScannerPage(ScannerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        // Solo codigos de productos (no QR ni 2D): menos lecturas falsas.
        Camara.BarcodeSymbologies = BarcodeFormats.Ean13 | BarcodeFormats.Ean8 | BarcodeFormats.Upca
            | BarcodeFormats.Upce | BarcodeFormats.Code128 | BarcodeFormats.Code39 | BarcodeFormats.Itf;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.Reanudar();
        _viewModel.AplicarPermiso(await Methods.AskForRequiredPermissionAsync());
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.Detener();
    }

    private async void Camara_OnDetectionFinished(object? sender, OnDetectionFinishedEventArg e)
    {
        string? codigo = e.BarcodeResults.FirstOrDefault()?.DisplayValue;
        if (string.IsNullOrWhiteSpace(codigo)) return;

        // El evento puede llegar fuera del hilo de UI.
        await MainThread.InvokeOnMainThreadAsync(() => _viewModel.CodigoLeidoAsync(codigo));
    }
}
