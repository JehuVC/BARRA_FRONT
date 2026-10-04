using BarraApp.Mobile.ViewModels.Usuario;

namespace BarraApp.Mobile.Views.Usuario;

public partial class RecuperarPage : ContentPage
{
    public RecuperarPage(RecuperarViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
