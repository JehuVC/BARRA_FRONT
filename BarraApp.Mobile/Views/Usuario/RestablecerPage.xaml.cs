using BarraApp.Mobile.ViewModels.Usuario;

namespace BarraApp.Mobile.Views.Usuario;

public partial class RestablecerPage : ContentPage
{
    public RestablecerPage(RestablecerViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
