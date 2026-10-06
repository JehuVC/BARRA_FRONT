using CommunityToolkit.Mvvm.ComponentModel;
using ProductoEntidad = Core.Entidades.Enti_Clases.Producto;

namespace BarraApp.Mobile.ViewModels.Producto;

// Un producto de los resultados de busqueda. Envuelve la entidad de Core y le agrega el estado
// que la pantalla cambia en vivo (favorito, operacion en curso).
public partial class ProductoItemViewModel(ProductoEntidad producto, bool favorito) : ObservableObject
{
    public ProductoEntidad Producto { get; } = producto;

    public Guid GuidProducto => Producto.guidProducto;
    public string CodigoBarras => Producto.codigoBarras ?? string.Empty;
    public string Nombre => Producto.nombre ?? string.Empty;
    public string Marca => string.IsNullOrWhiteSpace(Producto.marca) ? "Sin marca" : Producto.marca;
    public string Categoria => Producto.nombreCategoria ?? string.Empty;
    public bool TieneCategoria => !string.IsNullOrWhiteSpace(Producto.nombreCategoria);
    public string Inicial => string.IsNullOrWhiteSpace(Producto.nombre) ? "?" : Producto.nombre.Trim()[..1].ToUpperInvariant();

    [ObservableProperty] private bool esFavorito = favorito;
    [ObservableProperty] private bool procesando;

    public bool PuedeInteractuar => !Procesando;

    partial void OnProcesandoChanged(bool value) => OnPropertyChanged(nameof(PuedeInteractuar));
}
