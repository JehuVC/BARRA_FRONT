using BarraApp.Mobile.ViewModels.Historial;

namespace BarraApp.Mobile.Views.Historial;

public partial class HistorialPage : ContentPage
{
    private readonly HistorialViewModel _viewModel;

    public HistorialPage(HistorialViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    // Recarga cada vez que se vuelve a esta pantalla (el historial cambia al buscar o ver un producto).
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarCommand.ExecuteAsync(null);
    }
}
