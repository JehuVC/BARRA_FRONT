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
        // Ejecuta la llamada a la API cada vez que la pantalla aparece
        _viewModel.CargarHistorialCommand.Execute(null);
    }
}