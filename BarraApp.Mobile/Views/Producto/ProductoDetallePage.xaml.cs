using BarraApp.Mobile.ViewModels.Producto;

namespace BarraApp.Mobile.Views.Producto;

public partial class ProductoDetallePage : ContentPage
{
    private readonly ProductoDetalleViewModel _viewModel;

    public ProductoDetallePage(ProductoDetalleViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarCommand.ExecuteAsync(null);
    }
}
