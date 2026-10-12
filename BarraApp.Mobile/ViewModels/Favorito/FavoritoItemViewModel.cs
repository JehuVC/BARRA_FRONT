using System;
using BarraApp.Mobile.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using FavoritoEntidad = Core.Entidades.Enti_Clases.Favorito;

namespace BarraApp.Mobile.ViewModels.Favorito;

// Un producto de la lista de favoritos. Envuelve la entidad de Core y le agrega el estado que la pantalla cambia.
public partial class FavoritoItemViewModel(FavoritoEntidad favorito) : ObservableObject
{
    public FavoritoEntidad Favorito { get; } = favorito;

    public Guid GuidProducto => Favorito.guidProducto;
    public string CodigoBarras => Favorito.codigoBarras ?? string.Empty;
    public string Nombre => Favorito.nombreProducto ?? string.Empty;
    public string Marca => string.IsNullOrWhiteSpace(Favorito.marca) ? "Sin marca" : Favorito.marca;
    public string Inicial => string.IsNullOrWhiteSpace(Favorito.nombreProducto) ? "?" : Favorito.nombreProducto.Trim()[..1].ToUpperInvariant();

    public bool TieneFecha => Favorito.fechaAgregado.HasValue;
    public string Agregado => Favorito.fechaAgregado.HasValue ? $"Agregado {Formato.Hace(Favorito.fechaAgregado.Value)}" : string.Empty;

    [ObservableProperty] private bool procesando;
}