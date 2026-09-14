using CommunityToolkit.Mvvm.ComponentModel;

namespace BarraApp.Mobile.ViewModels.Dashboard;

public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty]
    private string tituloBienvenida = "BarraApp";
}
