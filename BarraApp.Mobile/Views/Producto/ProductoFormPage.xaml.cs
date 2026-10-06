using BarraApp.Mobile.ViewModels.Producto;

namespace BarraApp.Mobile.Views.Producto;

public partial class ProductoFormPage : ContentPage
{
    public ProductoFormPage(ProductoFormViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
