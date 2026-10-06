using BarraApp.Mobile.ViewModels.Producto;

namespace BarraApp.Mobile.Views.Producto;

public partial class BusquedaPage : ContentPage
{
    private readonly BusquedaViewModel _viewModel;

    public BusquedaPage(BusquedaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Si se llega sin un texto ya escrito, el teclado se abre solo para empezar a buscar.
        if (string.IsNullOrEmpty(_viewModel.TextoBusqueda))
        {
            Dispatcher.Dispatch(() => EntryBusqueda.Focus());
        }

        await _viewModel.CargarCommand.ExecuteAsync(null);
    }
}
