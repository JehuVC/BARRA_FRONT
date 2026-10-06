using System.Collections.ObjectModel;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Categoria;
using BarraApp.Mobile.Services.Favorito;
using BarraApp.Mobile.Services.Historial;
using BarraApp.Mobile.Services.ListaCompra;
using BarraApp.Mobile.Services.Producto;
using BarraApp.Mobile.Views.Producto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BarraApp.Mobile.ViewModels.Producto;

// Busqueda de productos por nombre o marca, con filtro por categoria. Desde cada resultado se puede
// marcar como favorito (api/favorito/*) o agregar a una lista de compra (api/itemlista/agregar).
// Cada busqueda con texto queda en el historial (api/historial/registrar).
public partial class BusquedaViewModel(
    IProductoService productoService,
    ICategoriaService categoriaService,
    IFavoritoService favoritoService,
    IHistorialService historialService,
    IAgregarAListaFlujo agregarAListaFlujo) : ObservableObject, IQueryAttributable
{
    private HashSet<Guid> _favoritos = new();
    private bool _buscarAlAparecer;

    [ObservableProperty] private string textoBusqueda = string.Empty;
    [ObservableProperty] private bool buscando;
    [ObservableProperty] private bool busquedaRealizada;
    [ObservableProperty] private string mensajeError = string.Empty;
    [ObservableProperty] private string mensajeExito = string.Empty;

    public ObservableCollection<ProductoItemViewModel> Resultados { get; } = new();
    public ObservableCollection<CategoriaChipViewModel> Categorias { get; } = new();

    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool HasSuccess => !string.IsNullOrWhiteSpace(MensajeExito);
    public bool HayTexto => !string.IsNullOrEmpty(TextoBusqueda);
    public bool HayCategorias => Categorias.Count > 1;
    public bool MostrarInicial => !BusquedaRealizada && !Buscando;
    public bool SinResultados => BusquedaRealizada && !Buscando && Resultados.Count == 0;
    public bool HayResultados => Resultados.Count > 0;
    public string Resumen => Resultados.Count == 1 ? "1 producto encontrado" : $"{Resultados.Count} productos encontrados";

    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));
    partial void OnMensajeExitoChanged(string value) => OnPropertyChanged(nameof(HasSuccess));
    partial void OnTextoBusquedaChanged(string value) => OnPropertyChanged(nameof(HayTexto));

    partial void OnBuscandoChanged(bool value)
    {
        OnPropertyChanged(nameof(MostrarInicial));
        OnPropertyChanged(nameof(SinResultados));
    }

    partial void OnBusquedaRealizadaChanged(bool value)
    {
        OnPropertyChanged(nameof(MostrarInicial));
        OnPropertyChanged(nameof(SinResultados));
    }

    private Guid? CategoriaActiva => Categorias.FirstOrDefault(c => c.Seleccionada)?.GuidCategoria;

    // Desde el Dashboard se llega con ?texto=... (la barra de busqueda o una busqueda reciente).
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("texto", out var texto) && !string.IsNullOrWhiteSpace(texto?.ToString()))
        {
            TextoBusqueda = Uri.UnescapeDataString(texto.ToString()!);
            _buscarAlAparecer = true;
        }
    }

    // Se corre cada vez que la pantalla aparece: carga las categorias una sola vez y refresca los
    // favoritos para que el corazon refleje cambios hechos en otras pantallas.
    [RelayCommand]
    public async Task CargarAsync()
    {
        MensajeError = string.Empty;

        try
        {
            if (Categorias.Count == 0)
            {
                var res = await categoriaService.ObtenerAsync();
                if (res.resultado)
                {
                    Categorias.Add(new CategoriaChipViewModel(null, "Todas") { Seleccionada = true });
                    foreach (var categoria in res.categorias ?? new())
                    {
                        Categorias.Add(new CategoriaChipViewModel(categoria.guid, categoria.nombre));
                    }

                    OnPropertyChanged(nameof(HayCategorias));
                }
            }

            await RefrescarFavoritosAsync();
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }

        if (_buscarAlAparecer)
        {
            _buscarAlAparecer = false;
            await BuscarAsync();
        }
    }

    [RelayCommand]
    private async Task BuscarAsync()
    {
        if (Buscando) return;
        MensajeError = string.Empty;

        string texto = TextoBusqueda.Trim();
        Guid? categoria = CategoriaActiva;

        if (texto.Length == 0 && categoria is null)
        {
            MensajeError = "Escribe qué producto buscas o elige una categoría.";
            return;
        }

        Buscando = true;
        try
        {
            var res = await productoService.BuscarAsync(texto, categoria);
            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudo realizar la búsqueda.");
                return;
            }

            Resultados.Clear();
            foreach (var producto in res.productos ?? new())
            {
                Resultados.Add(new ProductoItemViewModel(producto, _favoritos.Contains(producto.guidProducto)));
            }

            BusquedaRealizada = true;
            NotificarResultados();

            if (texto.Length > 0)
            {
                RegistrarEnHistorial(texto);
            }
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            Buscando = false;
        }
    }

    [RelayCommand]
    private async Task SeleccionarCategoriaAsync(CategoriaChipViewModel chip)
    {
        if (chip is null || Buscando) return;

        foreach (var c in Categorias)
        {
            c.Seleccionada = ReferenceEquals(c, chip);
        }

        // Con filtro y sin texto se busca igual; el chip "Todas" sin texto solo limpia los resultados.
        if (!string.IsNullOrWhiteSpace(TextoBusqueda) || chip.GuidCategoria is not null)
        {
            await BuscarAsync();
        }
        else
        {
            Resultados.Clear();
            BusquedaRealizada = false;
            NotificarResultados();
        }
    }

    [RelayCommand]
    private void LimpiarTexto()
    {
        TextoBusqueda = string.Empty;
    }

    [RelayCommand]
    private async Task AlternarFavoritoAsync(ProductoItemViewModel item)
    {
        if (item is null || item.Procesando) return;

        item.Procesando = true;
        MensajeError = string.Empty;
        try
        {
            if (item.EsFavorito)
            {
                var res = await favoritoService.QuitarAsync(item.GuidProducto);
                if (!res.resultado)
                {
                    MensajeError = MensajesError.Traducir(res.error, "No se pudo quitar de favoritos.");
                    return;
                }

                _favoritos.Remove(item.GuidProducto);
                item.EsFavorito = false;
                MostrarExito($"«{item.Nombre}» ya no está en tus favoritos.");
            }
            else
            {
                var res = await favoritoService.AgregarAsync(item.GuidProducto);
                if (!res.resultado)
                {
                    MensajeError = MensajesError.Traducir(res.error, "No se pudo agregar a favoritos.");
                    return;
                }

                _favoritos.Add(item.GuidProducto);
                item.EsFavorito = true;
                MostrarExito($"«{item.Nombre}» se agregó a tus favoritos.");
            }
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            item.Procesando = false;
        }
    }

    [RelayCommand]
    private async Task AgregarAListaAsync(ProductoItemViewModel item)
    {
        if (item is null || item.Procesando) return;

        item.Procesando = true;
        MensajeError = string.Empty;
        try
        {
            var r = await agregarAListaFlujo.EjecutarAsync(item.GuidProducto, item.Nombre);
            if (r.Error is not null) MensajeError = r.Error;
            else if (r.Exito is not null) MostrarExito(r.Exito);
        }
        finally
        {
            item.Procesando = false;
        }
    }

    [RelayCommand]
    private async Task AbrirDetalleAsync(ProductoItemViewModel item)
    {
        if (item is null || string.IsNullOrWhiteSpace(item.CodigoBarras)) return;

        await Shell.Current.GoToAsync(
            $"{nameof(ProductoDetallePage)}?codigo={Uri.EscapeDataString(item.CodigoBarras)}");
    }

    [RelayCommand]
    private async Task EscanearAsync()
    {
        await Shell.Current.GoToAsync(nameof(ScannerPage));
    }

    // Si el producto no esta en el catalogo, se registra; lo que el usuario escribio pasa como nombre sugerido.
    [RelayCommand]
    private async Task RegistrarProductoAsync()
    {
        string texto = TextoBusqueda.Trim();
        string ruta = texto.Length == 0
            ? nameof(ProductoFormPage)
            : $"{nameof(ProductoFormPage)}?texto={Uri.EscapeDataString(texto)}";

        await Shell.Current.GoToAsync(ruta);
    }

    [RelayCommand]
    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private async Task RefrescarFavoritosAsync()
    {
        var res = await favoritoService.ObtenerAsync();
        if (!res.resultado) return;

        _favoritos = (res.favoritos ?? new()).Select(f => f.guidProducto).ToHashSet();
        foreach (var item in Resultados)
        {
            item.EsFavorito = _favoritos.Contains(item.GuidProducto);
        }
    }

    // El historial es secundario: si falla (sin red, sesion vencida) no se le muestra nada al usuario.
    private void RegistrarEnHistorial(string texto)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await historialService.RegistrarBusquedaAsync(texto);
            }
            catch (Exception)
            {
            }
        });
    }

    private void NotificarResultados()
    {
        OnPropertyChanged(nameof(HayResultados));
        OnPropertyChanged(nameof(SinResultados));
        OnPropertyChanged(nameof(MostrarInicial));
        OnPropertyChanged(nameof(Resumen));
    }

    // Aviso breve que se oculta solo (no bloquea al usuario con una alerta).
    private async void MostrarExito(string mensaje)
    {
        MensajeExito = mensaje;
        await Task.Delay(2500);
        if (MensajeExito == mensaje)
        {
            MensajeExito = string.Empty;
        }
    }
}
