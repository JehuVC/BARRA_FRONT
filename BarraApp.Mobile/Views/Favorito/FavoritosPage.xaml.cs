using BarraApp.Mobile.ViewModels.Favorito;

namespace BarraApp.Mobile.Views.Favorito;

public partial class FavoritosPage : ContentPage
{
    private readonly FavoritosViewModel _viewModel;

    public FavoritosPage(FavoritosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    // Recarga cada vez que se vuelve a esta pantalla (ej. despues de quitar un favorito desde el detalle).
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarCommand.ExecuteAsync(null);
    }
}
