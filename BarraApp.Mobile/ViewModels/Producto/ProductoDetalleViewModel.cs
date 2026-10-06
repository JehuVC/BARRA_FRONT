using System.Collections.ObjectModel;
using BarraApp.Mobile.Helpers;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Comercio;
using BarraApp.Mobile.Services.Comparador;
using BarraApp.Mobile.Services.Favorito;
using BarraApp.Mobile.Services.Historial;
using BarraApp.Mobile.Services.ListaCompra;
using BarraApp.Mobile.Services.Producto;
using BarraApp.Mobile.Services.Ubicacion;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BarraApp.Mobile.Views.Producto;
using ProductoEntidad = Core.Entidades.Enti_Clases.Producto;

namespace BarraApp.Mobile.ViewModels.Producto;

// Detalle de un producto: sus datos, la comparacion de precios entre comercios (api/comparador/comparar),
// favorito, agregar a una lista y reportar un precio (api/producto/registrarPrecio).
public partial class ProductoDetalleViewModel(
    IProductoService productoService,
    IComparadorService comparadorService,
    IFavoritoService favoritoService,
    IHistorialService historialService,
    IComercioService comercioService,
    IUbicacionService ubicacionService,
    IAgregarAListaFlujo agregarAListaFlujo) : ObservableObject, IQueryAttributable
{
    // Radio con el que se buscan comercios cerca del usuario al reportar un precio.
    private const decimal RadioComerciosKm = 10;

    private string _codigoBarras = string.Empty;
    private Guid _guidProducto;
    private ProductoEntidad? _producto;
    private (decimal Latitud, decimal Longitud)? _ubicacion;
    private bool _productoCargado;
    private bool _historialRegistrado;

    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private string marca = string.Empty;
    [ObservableProperty] private string descripcion = string.Empty;
    [ObservableProperty] private string categoria = string.Empty;
    [ObservableProperty] private string unidad = string.Empty;
    [ObservableProperty] private string inicial = "?";

    [ObservableProperty] private bool estaCargando;
    [ObservableProperty] private bool procesando;
    [ObservableProperty] private bool esFavorito;
    [ObservableProperty] private bool tienePrecios;
    [ObservableProperty] private bool cargaTerminada;

    [ObservableProperty] private string mejorPrecioTexto = string.Empty;
    [ObservableProperty] private string mejorComercioTexto = string.Empty;
    [ObservableProperty] private string mejorAhorroTexto = string.Empty;

    [ObservableProperty] private string errorCarga = string.Empty;
    [ObservableProperty] private string mensajeError = string.Empty;
    [ObservableProperty] private string mensajeExito = string.Empty;

    public ObservableCollection<PrecioItemViewModel> Precios { get; } = new();

    public bool HayErrorCarga => !string.IsNullOrWhiteSpace(ErrorCarga);
    public bool MostrarContenido => _productoCargado && !HayErrorCarga;
    public bool SinPrecios => MostrarContenido && CargaTerminada && !TienePrecios;
    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool HasSuccess => !string.IsNullOrWhiteSpace(MensajeExito);
    public bool TieneDescripcion => !string.IsNullOrWhiteSpace(Descripcion);
    public bool TieneCategoria => !string.IsNullOrWhiteSpace(Categoria);
    public bool TieneUnidad => !string.IsNullOrWhiteSpace(Unidad);
    public bool TieneAhorroMejor => !string.IsNullOrWhiteSpace(MejorAhorroTexto);
    public bool PuedeInteractuar => !Procesando;
    public bool PuedeVerDistancias => MostrarContenido && TienePrecios && _ubicacion is null;
    public string TituloPrecios => Precios.Count == 1 ? "Precio registrado" : $"Comparar precios ({Precios.Count})";

    partial void OnErrorCargaChanged(string value)
    {
        OnPropertyChanged(nameof(HayErrorCarga));
        OnPropertyChanged(nameof(MostrarContenido));
    }

    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));
    partial void OnMensajeExitoChanged(string value) => OnPropertyChanged(nameof(HasSuccess));
    partial void OnProcesandoChanged(bool value) => OnPropertyChanged(nameof(PuedeInteractuar));
    partial void OnDescripcionChanged(string value) => OnPropertyChanged(nameof(TieneDescripcion));
    partial void OnCategoriaChanged(string value) => OnPropertyChanged(nameof(TieneCategoria));
    partial void OnUnidadChanged(string value) => OnPropertyChanged(nameof(TieneUnidad));
    partial void OnMejorAhorroTextoChanged(string value) => OnPropertyChanged(nameof(TieneAhorroMejor));
    partial void OnCargaTerminadaChanged(bool value) => OnPropertyChanged(nameof(SinPrecios));

    partial void OnTienePreciosChanged(bool value)
    {
        OnPropertyChanged(nameof(SinPrecios));
        OnPropertyChanged(nameof(PuedeVerDistancias));
    }

    // Se llega con ?codigo=<codigo de barras> desde la busqueda.
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("codigo", out var codigo) && !string.IsNullOrWhiteSpace(codigo?.ToString()))
        {
            _codigoBarras = Uri.UnescapeDataString(codigo.ToString()!);
        }
    }

    // La primera vez carga todo; las siguientes (al volver de otra pantalla) solo refresca el favorito.
    [RelayCommand]
    public async Task CargarAsync()
    {
        if (EstaCargando || string.IsNullOrWhiteSpace(_codigoBarras)) return;

        if (_productoCargado)
        {
            // Se vuelve de otra pantalla (ej. la edicion del producto): se refrescan los datos y el favorito.
            try
            {
                var actualizado = await productoService.ObtenerPorCodigoAsync(_codigoBarras);
                if (actualizado.resultado && actualizado.producto is not null)
                {
                    AplicarProducto(actualizado.producto);
                }

                await RefrescarFavoritoAsync();
            }
            catch (ApiException) { }
            return;
        }

        EstaCargando = true;
        ErrorCarga = string.Empty;
        MensajeError = string.Empty;

        try
        {
            var res = await productoService.ObtenerPorCodigoAsync(_codigoBarras);
            if (!res.resultado || res.producto is null)
            {
                ErrorCarga = MensajesError.Traducir(res.error, "No se pudo cargar el producto.");
                return;
            }

            AplicarProducto(res.producto);
            _productoCargado = true;
            OnPropertyChanged(nameof(MostrarContenido));

            // Si el usuario ya dio permiso de ubicacion antes, se muestran las distancias (sin volver a preguntar).
            _ubicacion = await ubicacionService.ObtenerAsync(pedirPermiso: false);

            await CargarPreciosAsync();
            await RefrescarFavoritoAsync();
            RegistrarEnHistorial();
        }
        catch (ApiException ex)
        {
            if (_productoCargado) MensajeError = ex.Message; else ErrorCarga = ex.Message;
        }
        finally
        {
            EstaCargando = false;
            CargaTerminada = true;
            OnPropertyChanged(nameof(MostrarContenido));
            OnPropertyChanged(nameof(SinPrecios));
        }
    }

    [RelayCommand]
    private async Task ReintentarAsync()
    {
        _productoCargado = false;
        await CargarAsync();
    }

    [RelayCommand]
    private async Task VerDistanciasAsync()
    {
        if (Procesando) return;
        Procesando = true;
        MensajeError = string.Empty;
        try
        {
            _ubicacion = await ubicacionService.ObtenerAsync(pedirPermiso: true);
            if (_ubicacion is null)
            {
                MensajeError = "No pudimos obtener tu ubicación. Activa el GPS y el permiso de ubicación para ver las distancias.";
                return;
            }

            await CargarPreciosAsync();
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            Procesando = false;
            OnPropertyChanged(nameof(PuedeVerDistancias));
        }
    }

    [RelayCommand]
    private async Task AlternarFavoritoAsync()
    {
        if (Procesando || _guidProducto == Guid.Empty) return;

        Procesando = true;
        MensajeError = string.Empty;
        try
        {
            if (EsFavorito)
            {
                var res = await favoritoService.QuitarAsync(_guidProducto);
                if (!res.resultado)
                {
                    MensajeError = MensajesError.Traducir(res.error, "No se pudo quitar de favoritos.");
                    return;
                }

                EsFavorito = false;
                MostrarExito("Quitado de tus favoritos.");
            }
            else
            {
                var res = await favoritoService.AgregarAsync(_guidProducto);
                if (!res.resultado)
                {
                    MensajeError = MensajesError.Traducir(res.error, "No se pudo agregar a favoritos.");
                    return;
                }

                EsFavorito = true;
                MostrarExito("Agregado a tus favoritos.");
            }
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            Procesando = false;
        }
    }

    [RelayCommand]
    private async Task AgregarAListaAsync()
    {
        if (Procesando || _guidProducto == Guid.Empty) return;

        Procesando = true;
        MensajeError = string.Empty;
        try
        {
            var r = await agregarAListaFlujo.EjecutarAsync(_guidProducto, Nombre);
            if (r.Error is not null) MensajeError = r.Error;
            else if (r.Exito is not null) MostrarExito(r.Exito);
        }
        finally
        {
            Procesando = false;
        }
    }

    // El usuario elige el comercio donde vio el precio (los cercanos a el y los que ya tienen precio de este
    // producto), escribe el monto y se registra; si el comercio ya tenia precio, se actualiza.
    [RelayCommand]
    private async Task ReportarPrecioAsync()
    {
        if (Procesando || _guidProducto == Guid.Empty) return;

        Procesando = true;
        MensajeError = string.Empty;
        try
        {
            var ubicacion = await ubicacionService.ObtenerAsync(pedirPermiso: true);
            if (ubicacion is not null && _ubicacion is null)
            {
                _ubicacion = ubicacion;
                OnPropertyChanged(nameof(PuedeVerDistancias));
            }

            var candidatos = new List<(Guid Guid, string Texto)>();

            if (ubicacion is { } u)
            {
                var cercanos = await comercioService.ObtenerCercanosAsync(u.Latitud, u.Longitud, RadioComerciosKm);
                if (cercanos.resultado)
                {
                    foreach (var c in cercanos.comercios ?? new())
                    {
                        string distancia = c.distanciaKm.HasValue ? " · " + Formato.Distancia(c.distanciaKm.Value) : string.Empty;
                        candidatos.Add((c.guidComercio, c.nombre + distancia));
                    }
                }
            }

            foreach (var p in Precios)
            {
                if (candidatos.All(c => c.Guid != p.GuidComercio))
                {
                    candidatos.Add((p.GuidComercio, p.Comercio));
                }
            }

            if (candidatos.Count == 0)
            {
                bool registrar = await Shell.Current.DisplayAlertAsync(
                    "Sin comercios cerca",
                    "No encontramos comercios cerca de ti. Para reportar un precio, el comercio debe estar registrado.",
                    "Registrar comercio",
                    "Ahora no");

                if (registrar)
                {
                    // Nombre de la ruta de ComercioFormPage (registrada en AppShell).
                    await Shell.Current.GoToAsync("ComercioFormPage");
                }

                return;
            }

            var opciones = candidatos.Select((c, i) => $"{i + 1}. {c.Texto}").ToArray();
            string? elegido = await Shell.Current.DisplayActionSheetAsync("¿Dónde viste este precio?", "Cancelar", null, opciones);
            int indice = Array.IndexOf(opciones, elegido);
            if (indice < 0) return;

            var comercio = candidatos[indice];
            string? texto = await Shell.Current.DisplayPromptAsync(
                "Reportar precio",
                $"¿Cuánto cuesta «{Nombre}» en {comercio.Texto.Split(" · ")[0]}? (en colones)",
                "Guardar",
                "Cancelar",
                placeholder: "Ej. 1850",
                maxLength: 12,
                keyboard: Keyboard.Numeric);

            if (texto is null) return;

            if (!Formato.TryParsePrecio(texto, out decimal precio) || precio <= 0 || precio > 99999999.99m)
            {
                MensajeError = "Escribe un precio válido, con máximo 2 decimales. Ej. 1850 o 1850.50";
                return;
            }

            var res = await productoService.RegistrarPrecioAsync(_guidProducto, comercio.Guid, precio);
            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudo registrar el precio.");
                return;
            }

            MostrarExito($"Precio guardado: {Formato.Colones(precio)}. ¡Gracias por aportar!");
            await CargarPreciosAsync();
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            Procesando = false;
        }
    }

    // Corrige los datos del producto (api/producto/actualizar). Se pasa la entidad completa porque el formulario
    // necesita tambien los guid de categoria y unidad.
    [RelayCommand]
    private async Task EditarAsync()
    {
        if (Procesando || _producto is null) return;

        await Shell.Current.GoToAsync(nameof(ProductoFormPage),
            new ShellNavigationQueryParameters { { "producto", _producto } });
    }

    [RelayCommand]
    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private void AplicarProducto(ProductoEntidad p)
    {
        _producto = p;
        _guidProducto = p.guidProducto;
        Nombre = p.nombre ?? string.Empty;
        Marca = string.IsNullOrWhiteSpace(p.marca) ? "Sin marca" : p.marca;
        Descripcion = p.descripcion ?? string.Empty;
        Categoria = p.nombreCategoria ?? string.Empty;
        Unidad = p.nombreUnidad ?? string.Empty;
        Inicial = string.IsNullOrWhiteSpace(p.nombre) ? "?" : p.nombre.Trim()[..1].ToUpperInvariant();
    }

    private async Task CargarPreciosAsync()
    {
        var res = await comparadorService.CompararAsync(_codigoBarras, _ubicacion?.Latitud, _ubicacion?.Longitud);
        if (!res.resultado)
        {
            MensajeError = MensajesError.Traducir(res.error, "No se pudieron cargar los precios.");
            return;
        }

        var precios = res.precios ?? new();

        Precios.Clear();
        for (int i = 0; i < precios.Count; i++)
        {
            Precios.Add(new PrecioItemViewModel(precios[i], esMejor: i == 0));
        }

        TienePrecios = Precios.Count > 0;

        if (TienePrecios)
        {
            var mejor = Precios[0];
            MejorPrecioTexto = mejor.PrecioTexto;
            MejorComercioTexto = $"en {mejor.Comercio}";
            MejorAhorroTexto = mejor.Precio.montoDescuento > 0 ? mejor.Ahorro : string.Empty;
        }
        else
        {
            MejorPrecioTexto = MejorComercioTexto = MejorAhorroTexto = string.Empty;
        }

        OnPropertyChanged(nameof(TituloPrecios));
        OnPropertyChanged(nameof(SinPrecios));
        OnPropertyChanged(nameof(PuedeVerDistancias));
    }

    private async Task RefrescarFavoritoAsync()
    {
        var res = await favoritoService.ObtenerAsync();
        if (res.resultado)
        {
            EsFavorito = (res.favoritos ?? new()).Any(f => f.guidProducto == _guidProducto);
        }
    }

    // Ver el detalle de un producto cuenta como busqueda en el historial. Es secundario: si falla no se avisa.
    private void RegistrarEnHistorial()
    {
        if (_historialRegistrado || _guidProducto == Guid.Empty) return;
        _historialRegistrado = true;

        string termino = Nombre;
        Guid guid = _guidProducto;
        _ = Task.Run(async () =>
        {
            try
            {
                await historialService.RegistrarBusquedaAsync(termino, guid);
            }
            catch (Exception)
            {
            }
        });
    }

    // Aviso breve que se oculta solo.
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
