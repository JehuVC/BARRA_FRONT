using BarraApp.Mobile.ViewModels.ListaCompra;

namespace BarraApp.Mobile.Views.ListaCompra;

public partial class ListaDetallePage : ContentPage
{
    private readonly ListaDetalleViewModel _viewModel;

    public ListaDetallePage(ListaDetalleViewModel viewModel)
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
