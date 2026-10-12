using BarraApp.Mobile.ViewModels.Dashboard;

namespace BarraApp.Mobile.Views.Dashboard;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Asegura que el menú lateral esté cerrado al mostrar la página
        _viewModel.MenuLateralVisible = false;
        _viewModel.CargarHistorialCommand.Execute(null);
        _viewModel.CargarNombreCommand.Execute(null);
    }
}