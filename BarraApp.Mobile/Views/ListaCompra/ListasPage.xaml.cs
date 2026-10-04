using BarraApp.Mobile.ViewModels.ListaCompra;

namespace BarraApp.Mobile.Views.ListaCompra;

public partial class ListasPage : ContentPage
{
    private readonly ListasViewModel _viewModel;

    public ListasPage(ListasViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    // Recarga cada vez que se vuelve a esta pantalla (ej. al regresar del detalle de una lista).
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarCommand.ExecuteAsync(null);
    }
}
