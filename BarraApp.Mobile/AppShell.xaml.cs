using BarraApp.Mobile.Views.Comercio;
using BarraApp.Mobile.Views.Favorito;
using BarraApp.Mobile.Views.Guild;
using BarraApp.Mobile.Views.Historial;
using BarraApp.Mobile.Views.ListaCompra;
using BarraApp.Mobile.Views.Pago;
using BarraApp.Mobile.Views.Producto;
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
        Routing.RegisterRoute(nameof(BusquedaPage), typeof(BusquedaPage));
        Routing.RegisterRoute(nameof(ProductoDetallePage), typeof(ProductoDetallePage));
        Routing.RegisterRoute(nameof(ProductoFormPage), typeof(ProductoFormPage));
        Routing.RegisterRoute(nameof(PagoPage), typeof(PagoPage));
        Routing.RegisterRoute(nameof(ScannerPage), typeof(ScannerPage));
        Routing.RegisterRoute(nameof(FavoritosPage), typeof(FavoritosPage));
        Routing.RegisterRoute(nameof(HistorialPage), typeof(HistorialPage));
        Routing.RegisterRoute(nameof(ComerciosPage), typeof(ComerciosPage));
        Routing.RegisterRoute(nameof(ComercioFormPage), typeof(ComercioFormPage));
    }
}
