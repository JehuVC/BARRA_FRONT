using BarraApp.Mobile.Views.Usuario;

namespace BarraApp.Mobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(RegistroPage), typeof(RegistroPage));
    }
}
