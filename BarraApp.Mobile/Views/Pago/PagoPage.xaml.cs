using BarraApp.Mobile.ViewModels.Pago;

namespace BarraApp.Mobile.Views.Pago;

public partial class PagoPage : ContentPage
{
    private readonly PagoViewModel _viewModel;

    public PagoPage(PagoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarCommand.ExecuteAsync(null);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.Detener();
    }
}
