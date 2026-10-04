using BarraApp.Mobile.Views.Guild;
using BarraApp.Mobile.Views.ListaCompra;
using BarraApp.Mobile.Views.Usuario;

namespace BarraApp.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(RegistroPage), typeof(RegistroPage));
        Routing.RegisterRoute(nameof(PerfilPage), typeof(PerfilPage));

        // Pantallas de detalle: se llega empujando desde otra pagina, nunca como raiz del Shell.
        Routing.RegisterRoute(nameof(RecuperarPage), typeof(RecuperarPage));
        Routing.RegisterRoute(nameof(RestablecerPage), typeof(RestablecerPage));
        Routing.RegisterRoute(nameof(GuildPage), typeof(GuildPage));
        Routing.RegisterRoute(nameof(ListasPage), typeof(ListasPage));
        Routing.RegisterRoute(nameof(ListaDetallePage), typeof(ListaDetallePage));
    }
}
