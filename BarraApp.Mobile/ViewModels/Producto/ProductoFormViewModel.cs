using BarraApp.Mobile.Helpers;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Categoria;
using BarraApp.Mobile.Services.Producto;
using BarraApp.Mobile.Services.Unidad;
using BarraApp.Mobile.Views.Producto;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DTO.Producto;
using CategoriaEntidad = Core.Entidades.Enti_Clases.Categoria;
using ProductoEntidad = Core.Entidades.Enti_Clases.Producto;
using UnidadEntidad = Core.Entidades.Enti_Clases.Unidad;

namespace BarraApp.Mobile.ViewModels.Producto;

// Crear (api/producto/crear) o editar (api/producto/actualizar) un producto del catalogo. Para editar se llega con la
// entidad completa en el parametro "producto"; para crear se puede llegar con "codigo" (ej. del escaner) y/o "texto"
// (lo que el usuario buscaba) para adelantar el formulario. La categoria y la unidad se eligen de las existentes
// (api/categoria/obtener y api/unidad/obtener) o se crean al momento.
public partial class ProductoFormViewModel(
    IProductoService productoService,
    ICategoriaService categoriaService,
    IUnidadService unidadService) : ObservableObject, IQueryAttributable
{
    private const string SinCategoria = "Sin categoría";
    private const string SinUnidad = "Sin unidad";
    private const string NuevaCategoria = "➕ Nueva categoría…";
    private const string NuevaUnidad = "➕ Nueva unidad…";

    private Guid _guidProducto = Guid.Empty;
    private Guid? _guidCategoria;
    private Guid? _guidUnidad;
    private List<CategoriaEntidad>? _categorias;
    private List<UnidadEntidad>? _unidades;

    [ObservableProperty] private string codigoBarras = string.Empty;
    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private string marca = string.Empty;
    [ObservableProperty] private string descripcion = string.Empty;
    [ObservableProperty] private string categoriaTexto = SinCategoria;
    [ObservableProperty] private string unidadTexto = SinUnidad;
    [ObservableProperty] private string mensajeError = string.Empty;
    [ObservableProperty] private bool estaOcupado;

    public bool EsEdicion => _guidProducto != Guid.Empty;
    public string Titulo => EsEdicion ? "Editar producto" : "Nuevo producto";
    public string TextoGuardar => EsEdicion ? "Guardar cambios" : "Registrar producto";
    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool PuedeInteractuar => !EstaOcupado;
    public bool PuedeEditarCodigo => !EsEdicion && !EstaOcupado;

    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));

    partial void OnEstaOcupadoChanged(bool value)
    {
        OnPropertyChanged(nameof(PuedeInteractuar));
        OnPropertyChanged(nameof(PuedeEditarCodigo));
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("producto", out var valor) && valor is ProductoEntidad p)
        {
            _guidProducto = p.guidProducto;
            _guidCategoria = p.guidCategoria;
            _guidUnidad = p.guidUnidad;
            CodigoBarras = p.codigoBarras ?? string.Empty;
            Nombre = p.nombre ?? string.Empty;
            Marca = p.marca ?? string.Empty;
            Descripcion = p.descripcion ?? string.Empty;
            CategoriaTexto = string.IsNullOrWhiteSpace(p.nombreCategoria) ? SinCategoria : p.nombreCategoria;
            UnidadTexto = string.IsNullOrWhiteSpace(p.nombreUnidad) ? SinUnidad : p.nombreUnidad;

            OnPropertyChanged(nameof(EsEdicion));
            OnPropertyChanged(nameof(Titulo));
            OnPropertyChanged(nameof(TextoGuardar));
            OnPropertyChanged(nameof(PuedeEditarCodigo));
            return;
        }

        if (query.TryGetValue("codigo", out var codigo) && !string.IsNullOrWhiteSpace(codigo?.ToString()))
        {
            CodigoBarras = Uri.UnescapeDataString(codigo.ToString()!).Trim();
        }

        if (query.TryGetValue("texto", out var texto) && !string.IsNullOrWhiteSpace(texto?.ToString()))
        {
            Nombre = Uri.UnescapeDataString(texto.ToString()!).Trim();
        }
    }

    [RelayCommand]
    private async Task ElegirCategoriaAsync()
    {
        if (EstaOcupado) return;
        EstaOcupado = true;
        MensajeError = string.Empty;
        try
        {
            if (!await AsegurarCatalogosAsync()) return;

            var nombres = _categorias!.Select(c => c.nombre).ToList();
            var opciones = new[] { SinCategoria }.Concat(nombres).Append(NuevaCategoria).ToArray();
            string? elegido = await Shell.Current.DisplayActionSheetAsync("Categoría", "Cancelar", null, opciones);

            if (elegido is null || elegido == "Cancelar") return;

            if (elegido == SinCategoria)
            {
                _guidCategoria = null;
                CategoriaTexto = SinCategoria;
            }
            else if (elegido == NuevaCategoria)
            {
                await CrearCategoriaAsync();
            }
            else
            {
                var categoria = _categorias!.FirstOrDefault(c => c.nombre == elegido);
                if (categoria is not null)
                {
                    _guidCategoria = categoria.guid;
                    CategoriaTexto = categoria.nombre;
                }
            }
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    [RelayCommand]
    private async Task ElegirUnidadAsync()
    {
        if (EstaOcupado) return;
        EstaOcupado = true;
        MensajeError = string.Empty;
        try
        {
            if (!await AsegurarCatalogosAsync()) return;

            var nombres = _unidades!.Select(NombreUnidad).ToList();
            var opciones = new[] { SinUnidad }.Concat(nombres).Append(NuevaUnidad).ToArray();
            string? elegido = await Shell.Current.DisplayActionSheetAsync("Unidad de medida", "Cancelar", null, opciones);

            if (elegido is null || elegido == "Cancelar") return;

            if (elegido == SinUnidad)
            {
                _guidUnidad = null;
                UnidadTexto = SinUnidad;
            }
            else if (elegido == NuevaUnidad)
            {
                await CrearUnidadAsync();
            }
            else
            {
                var unidad = _unidades!.FirstOrDefault(u => NombreUnidad(u) == elegido);
                if (unidad is not null)
                {
                    _guidUnidad = unidad.guidUnidad;
                    UnidadTexto = unidad.nombre;
                }
            }
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (EstaOcupado) return;
        MensajeError = string.Empty;

        string? error = Validar();
        if (error is not null)
        {
            MensajeError = error;
            return;
        }

        EstaOcupado = true;
        try
        {
            string codigo = CodigoBarras.Trim();
            string nombreLimpio = Nombre.Trim();
            string? marcaLimpia = Vacio(Marca);
            string? descripcionLimpia = Vacio(Descripcion);

            if (EsEdicion)
            {
                var res = await productoService.ActualizarAsync(new DTOActualizarProducto
                {
                    guidProducto = _guidProducto,
                    nombre = nombreLimpio,
                    marca = marcaLimpia!,
                    descripcion = descripcionLimpia!,
                    guidCategoria = _guidCategoria,
                    guidUnidad = _guidUnidad
                });

                if (!res.resultado)
                {
                    MensajeError = MensajesError.Traducir(res.error, "No se pudo guardar el producto.");
                    return;
                }

                await Shell.Current.DisplayAlertAsync("Listo", "Los datos del producto se actualizaron.", "OK");
                await Shell.Current.GoToAsync("..");
                return;
            }

            if (!await ConfirmarSiHayParecidoAsync(nombreLimpio, marcaLimpia))
            {
                return;
            }

            var creado = await productoService.CrearAsync(new DTOCrearProducto
            {
                codigoBarras = codigo,
                nombre = nombreLimpio,
                marca = marcaLimpia!,
                descripcion = descripcionLimpia!,
                guidCategoria = _guidCategoria,
                guidUnidad = _guidUnidad
            });

            if (!creado.resultado)
            {
                if (creado.error?.Any(e => e.codigo == Core.Enums.enumErrores.codigoBarrasDuplicado) == true)
                {
                    await OfrecerVerExistenteAsync(codigo);
                    return;
                }

                MensajeError = MensajesError.Traducir(creado.error, "No se pudo registrar el producto.");
                return;
            }

            bool ver = await Shell.Current.DisplayAlertAsync(
                "Producto registrado",
                $"«{nombreLimpio}» ya está en el catálogo. ¡Gracias por aportar! ¿Quieres ver su ficha para reportar un precio?",
                "Ver producto",
                "Listo");

            await Shell.Current.GoToAsync("..");
            if (ver)
            {
                await Shell.Current.GoToAsync($"{nameof(ProductoDetallePage)}?codigo={Uri.EscapeDataString(codigo)}");
            }
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            EstaOcupado = false;
        }
    }

    [RelayCommand]
    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    // Mismos maximos que el backend (Logica de producto): son el tamano de cada columna.
    private string? Validar()
    {
        if (!EsEdicion)
        {
            string codigo = CodigoBarras.Trim();
            if (codigo.Length == 0) return "Ingresa el código de barras del producto.";
            if (codigo.Length > 50) return "El código de barras es demasiado largo (máximo 50 caracteres).";
            if (codigo.Any(char.IsWhiteSpace)) return "El código de barras no debe llevar espacios.";
        }

        if (string.IsNullOrWhiteSpace(Nombre)) return "Ingresa el nombre del producto.";
        if (Nombre.Trim().Length > 150) return "El nombre es demasiado largo (máximo 150 caracteres).";
        if (Marca.Trim().Length > 100) return "La marca es demasiado larga (máximo 100 caracteres).";
        if (Descripcion.Trim().Length > 500) return "La descripción es demasiado larga (máximo 500 caracteres).";

        return null;
    }

    // Avisa si ya hay un producto con nombre y marca parecidos (con la misma marca, o ambos sin marca): en un
    // comparador de precios un duplicado reparte los precios entre dos fichas.
    private async Task<bool> ConfirmarSiHayParecidoAsync(string nombreLimpio, string? marcaLimpia)
    {
        try
        {
            var res = await productoService.BuscarAsync(nombreLimpio);
            if (!res.resultado) return true;

            var parecido = (res.productos ?? new()).FirstOrDefault(p =>
                Formato.NombresParecidos(p.nombre, nombreLimpio)
                && (string.IsNullOrWhiteSpace(p.marca) && marcaLimpia is null || Formato.NombresParecidos(p.marca, marcaLimpia)));

            if (parecido is null) return true;

            string marca = string.IsNullOrWhiteSpace(parecido.marca) ? string.Empty : $" ({parecido.marca})";
            return await Shell.Current.DisplayAlertAsync(
                "¿Ya está en el catálogo?",
                $"Ya existe «{parecido.nombre}»{marca}. Si es el mismo producto, búscalo y repórtale el precio en vez de crear otro.",
                "Crear de todos modos",
                "Cancelar");
        }
        catch (ApiException)
        {
            // La comprobacion es una ayuda: si falla, el backend igual valida el codigo de barras.
            return true;
        }
    }

    private async Task OfrecerVerExistenteAsync(string codigo)
    {
        bool ver = await Shell.Current.DisplayAlertAsync(
            "Ese código ya existe",
            "Ya hay un producto registrado con ese código de barras. ¿Quieres verlo?",
            "Ver producto",
            "Cerrar");

        if (ver)
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync($"{nameof(ProductoDetallePage)}?codigo={Uri.EscapeDataString(codigo)}");
        }
    }

    // Carga las categorias y unidades la primera vez que se necesitan. Devuelve false (y deja el error) si falla.
    private async Task<bool> AsegurarCatalogosAsync()
    {
        if (_categorias is null)
        {
            var res = await categoriaService.ObtenerAsync();
            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudieron cargar las categorías.");
                return false;
            }

            _categorias = res.categorias ?? new();
        }

        if (_unidades is null)
        {
            var res = await unidadService.ObtenerAsync();
            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudieron cargar las unidades.");
                return false;
            }

            _unidades = res.unidades ?? new();
        }

        return true;
    }

    private async Task CrearCategoriaAsync()
    {
        string? texto = await Shell.Current.DisplayPromptAsync(
            "Nueva categoría",
            "¿Cómo se llama la categoría? Ej. Lácteos",
            "Crear",
            "Cancelar",
            maxLength: 100);

        if (texto is null) return;

        string nombreCategoria = texto.Trim();
        if (nombreCategoria.Length == 0)
        {
            MensajeError = "Ingresa el nombre de la categoría.";
            return;
        }

        var existente = _categorias!.FirstOrDefault(c => Formato.NombresIguales(c.nombre, nombreCategoria));
        if (existente is not null)
        {
            _guidCategoria = existente.guid;
            CategoriaTexto = existente.nombre;
            return;
        }

        var res = await categoriaService.CrearAsync(nombreCategoria);
        if (!res.resultado || res.categoria is null)
        {
            MensajeError = MensajesError.Traducir(res.error, "No se pudo crear la categoría.");
            return;
        }

        _categorias!.Add(res.categoria);
        _guidCategoria = res.categoria.guid;
        CategoriaTexto = res.categoria.nombre;
    }

    private async Task CrearUnidadAsync()
    {
        string? texto = await Shell.Current.DisplayPromptAsync(
            "Nueva unidad",
            "¿Cómo se llama la unidad? Ej. Litro",
            "Siguiente",
            "Cancelar",
            maxLength: 50);

        if (texto is null) return;

        string nombreUnidad = texto.Trim();
        if (nombreUnidad.Length == 0)
        {
            MensajeError = "Ingresa el nombre de la unidad.";
            return;
        }

        var existente = _unidades!.FirstOrDefault(u => Formato.NombresIguales(u.nombre, nombreUnidad));
        if (existente is not null)
        {
            _guidUnidad = existente.guidUnidad;
            UnidadTexto = existente.nombre;
            return;
        }

        string? abreviatura = await Shell.Current.DisplayPromptAsync(
            "Abreviatura",
            $"¿Cómo se abrevia «{nombreUnidad}»? Ej. L (puedes dejarlo vacío)",
            "Crear",
            "Cancelar",
            maxLength: 10);

        if (abreviatura is null) return;

        var res = await unidadService.CrearAsync(nombreUnidad, Vacio(abreviatura));
        if (!res.resultado || res.unidad is null)
        {
            MensajeError = MensajesError.Traducir(res.error, "No se pudo crear la unidad.");
            return;
        }

        _unidades!.Add(res.unidad);
        _guidUnidad = res.unidad.guidUnidad;
        UnidadTexto = res.unidad.nombre;
    }

    private static string NombreUnidad(UnidadEntidad u) =>
        string.IsNullOrWhiteSpace(u.abreviatura) ? u.nombre : $"{u.nombre} ({u.abreviatura})";

    private static string? Vacio(string texto) => string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
}
