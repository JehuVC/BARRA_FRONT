using BarraApp.Mobile.ViewModels.Usuario;

namespace BarraApp.Mobile.Views.Usuario;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
