using BarraApp.Mobile.ViewModels.Comercio;

namespace BarraApp.Mobile.Views.Comercio;

public partial class ComercioFormPage : ContentPage
{
    public ComercioFormPage(ComercioFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
