using BarraApp.Mobile.ViewModels.Usuario;

namespace BarraApp.Mobile.Views.Usuario;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InicializarCommand.ExecuteAsync(null);
    }
}
