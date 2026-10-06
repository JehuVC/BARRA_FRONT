using BarraApp.Mobile.ViewModels.Comercio;

namespace BarraApp.Mobile.Views.Comercio;

public partial class ComerciosPage : ContentPage
{
    private readonly ComerciosViewModel _viewModel;

    public ComerciosPage(ComerciosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    // Recarga cada vez que se vuelve a esta pantalla (ej. despues de registrar o editar un comercio).
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarCommand.ExecuteAsync(null);
    }
}
