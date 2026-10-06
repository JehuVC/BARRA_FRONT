using BarraApp.Mobile.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using ComercioEntidad = Core.Entidades.Enti_Clases.Comercio;

namespace BarraApp.Mobile.ViewModels.Comercio;

// Un comercio de la lista de cercanos. Envuelve la entidad de Core (que se pasa completa al formulario de edicion).
public class ComercioItemViewModel(ComercioEntidad comercio)
{
    public ComercioEntidad Comercio { get; } = comercio;

    public Guid GuidComercio => Comercio.guidComercio;
    public string Nombre => Comercio.nombre ?? string.Empty;

    public string Cadena => Comercio.cadena ?? string.Empty;
    public bool TieneCadena => !string.IsNullOrWhiteSpace(Comercio.cadena);

    public string Direccion => Comercio.direccion ?? string.Empty;
    public bool TieneDireccion => !string.IsNullOrWhiteSpace(Comercio.direccion);

    public string Horario => "🕒 " + Comercio.horario;
    public bool TieneHorario => !string.IsNullOrWhiteSpace(Comercio.horario);

    public string Telefono => "📞 " + Comercio.telefono;
    public bool TieneTelefono => !string.IsNullOrWhiteSpace(Comercio.telefono);

    public bool TieneDistancia => Comercio.distanciaKm.HasValue;
    public string Distancia => Comercio.distanciaKm.HasValue ? Formato.Distancia(Comercio.distanciaKm.Value) : string.Empty;
}

// Un radio de busqueda como "chip" (1, 3, 5, 10, 25 km).
public partial class RadioChipViewModel(decimal km) : ObservableObject
{
    public decimal Km { get; } = km;
    public string Texto => $"{Km:0.##} km";

    [ObservableProperty] private bool seleccionado;
}
