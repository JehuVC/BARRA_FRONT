using CommunityToolkit.Mvvm.ComponentModel;

namespace BarraApp.Mobile.ViewModels.Producto;

// Una categoria como "chip" de filtro. GuidCategoria null = el chip "Todas".
public partial class CategoriaChipViewModel(Guid? guidCategoria, string nombre) : ObservableObject
{
    public Guid? GuidCategoria { get; } = guidCategoria;
    public string Nombre { get; } = nombre;

    [ObservableProperty] private bool seleccionada;
}
