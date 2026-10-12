using System.Globalization;
using BarraApp.Mobile.Helpers;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Comercio;
using BarraApp.Mobile.Services.Ubicacion;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DTO.Comercio;
using ComercioEntidad = Core.Entidades.Enti_Clases.Comercio;

namespace BarraApp.Mobile.ViewModels.Comercio;

// Crear (api/comercio/crear) o editar (api/comercio/actualizar) un comercio. Para editar se llega con la entidad
// completa en el parametro "comercio"; sin parametro es un comercio nuevo.
public partial class ComercioFormViewModel(IComercioService comercioService, IUbicacionService ubicacionService)
    : ObservableObject, IQueryAttributable
{
    // Si ya hay un comercio con nombre parecido a menos de esta distancia, se avisa antes de crear otro.
    private const decimal RadioDuplicadosKm = 0.3m;

    private Guid _guidComercio = Guid.Empty;

    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private string cadena = string.Empty;
    [ObservableProperty] private string direccion = string.Empty;
    [ObservableProperty] private string telefono = string.Empty;
    [ObservableProperty] private string horario = string.Empty;
    [ObservableProperty] private string latitudTexto = string.Empty;
    [ObservableProperty] private string longitudTexto = string.Empty;
    [ObservableProperty] private string mensajeError = string.Empty;
    [ObservableProperty] private bool estaOcupado;

    public bool EsEdicion => _guidComercio != Guid.Empty;
    public string Titulo => EsEdicion ? "Editar comercio" : "Nuevo comercio";
    public string TextoGuardar => EsEdicion ? "Guardar cambios" : "Registrar comercio";
    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool PuedeInteractuar => !EstaOcupado;

    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));
    partial void OnEstaOcupadoChanged(bool value) => OnPropertyChanged(nameof(PuedeInteractuar));

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("comercio", out var valor) && valor is ComercioEntidad c)
        {
            _guidComercio = c.guidComercio;
            Nombre = c.nombre ?? string.Empty;
            Cadena = c.cadena ?? string.Empty;
            Direccion = c.direccion ?? string.Empty;
            Telefono = c.telefono ?? string.Empty;
            Horario = c.horario ?? string.Empty;
            LatitudTexto = c.latitud.ToString(CultureInfo.InvariantCulture);
            LongitudTexto = c.longitud.ToString(CultureInfo.InvariantCulture);

            OnPropertyChanged(nameof(EsEdicion));
            OnPropertyChanged(nameof(Titulo));
            OnPropertyChanged(nameof(TextoGuardar));
        }
    }

    [RelayCommand]
    private async Task UsarMiUbicacionAsync()
    {
        if (EstaOcupado) return;
        EstaOcupado = true;
        MensajeError = string.Empty;
        try
        {
            var ubicacion = await ubicacionService.ObtenerAsync(pedirPermiso: true);
            if (ubicacion is not { } u)
            {
                MensajeError = "No pudimos obtener tu ubicación. Activa el GPS y el permiso de ubicación, o escribe las coordenadas a mano.";
                return;
            }

            LatitudTexto = u.Latitud.ToString(CultureInfo.InvariantCulture);
            LongitudTexto = u.Longitud.ToString(CultureInfo.InvariantCulture);
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

        string? error = Validar(out decimal latitud, out decimal longitud);
        if (error is not null)
        {
            MensajeError = error;
            return;
        }

        EstaOcupado = true;
        try
        {
            if (!EsEdicion && !await ConfirmarSiHayDuplicadoAsync(latitud, longitud))
            {
                return;
            }

            string nombreLimpio = Nombre.Trim();
            string? cadenaLimpia = Vacio(Cadena);
            string direccionLimpia = Direccion.Trim();
            string? telefonoLimpio = Vacio(Telefono);
            string? horarioLimpio = Vacio(Horario);

            Core.Entidades.Response.ResBase res;
            if (EsEdicion)
            {
                res = await comercioService.ActualizarAsync(new DTOActualizarComercio
                {
                    guidComercio = _guidComercio,
                    nombre = nombreLimpio,
                    cadena = cadenaLimpia!,
                    direccion = direccionLimpia,
                    latitud = latitud,
                    longitud = longitud,
                    telefono = telefonoLimpio!,
                    horario = horarioLimpio!
                });
            }
            else
            {
                res = await comercioService.CrearAsync(new DTOCrearComercio
                {
                    nombre = nombreLimpio,
                    cadena = cadenaLimpia!,
                    direccion = direccionLimpia,
                    latitud = latitud,
                    longitud = longitud,
                    telefono = telefonoLimpio!,
                    horario = horarioLimpio!
                });
            }

            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudo guardar el comercio.");
                return;
            }

            await Shell.Current.DisplayAlertAsync(
                "Listo",
                EsEdicion ? "Los datos del comercio se actualizaron." : "El comercio quedó registrado. ¡Gracias por aportar!",
                "OK");

            await Shell.Current.GoToAsync("..");
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

    // Misma regla que el backend (Logica de comercio): los maximos son el tamano de cada columna.
    private string? Validar(out decimal latitud, out decimal longitud)
    {
        latitud = longitud = 0;

        if (string.IsNullOrWhiteSpace(Nombre)) return "Ingresa el nombre del comercio.";
        if (Nombre.Trim().Length > 150) return "El nombre es demasiado largo (máximo 150 caracteres).";
        if (Cadena.Trim().Length > 100) return "La cadena es demasiado larga (máximo 100 caracteres).";
        if (string.IsNullOrWhiteSpace(Direccion)) return "Ingresa la dirección del comercio.";
        if (Direccion.Trim().Length > 255) return "La dirección es demasiado larga (máximo 255 caracteres).";
        if (Telefono.Trim().Length > 20) return "El teléfono es demasiado largo (máximo 20 caracteres).";
        if (Horario.Trim().Length > 255) return "El horario es demasiado largo (máximo 255 caracteres).";

        if (!Formato.TryParseCoordenada(LatitudTexto, out latitud) || !Formato.TryParseCoordenada(LongitudTexto, out longitud))
        {
            return "Indica la ubicación del comercio: toca «Usar mi ubicación actual» o escribe la latitud y la longitud.";
        }

        if (latitud < -90 || latitud > 90) return "La latitud debe estar entre -90 y 90.";
        if (longitud < -180 || longitud > 180) return "La longitud debe estar entre -180 y 180.";
        if (latitud == 0 && longitud == 0) return "La ubicación 0, 0 no es válida: usa tu ubicación actual o las coordenadas reales.";

        return null;
    }

    private async Task<bool> ConfirmarSiHayDuplicadoAsync(decimal latitud, decimal longitud)
    {
        var cercanos = await comercioService.ObtenerCercanosAsync(latitud, longitud, RadioDuplicadosKm);
        if (!cercanos.resultado) return true;

        var parecido = (cercanos.comercios ?? new()).FirstOrDefault(c => Formato.NombresParecidos(c.nombre, Nombre));
        if (parecido is null) return true;

        string distancia = parecido.distanciaKm.HasValue ? $" {Formato.Distancia(parecido.distanciaKm.Value)}" : string.Empty;
        return await Shell.Current.DisplayAlertAsync(
            "¿Ya está registrado?",
            $"Ya existe «{parecido.nombre}»{distancia} de esa ubicación. Si es el mismo comercio, edítalo desde la lista en vez de crear otro.",
            "Crear de todos modos",
            "Cancelar");
    }

    private static string? Vacio(string texto) => string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
}