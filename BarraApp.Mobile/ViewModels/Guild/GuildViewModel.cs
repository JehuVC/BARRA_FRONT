using System.Collections.ObjectModel;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.Guild;
using BarraApp.Mobile.Services.Sesion;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Entidades.Enti_Clases;

namespace BarraApp.Mobile.ViewModels.Guild;

// FE-S7-08 (crear gremio), FE-S7-09 (unirse con codigo) y FE-S7-10 (ver miembros y salir).
// La pantalla tiene dos estados segun el perfil del usuario: sin gremio (crear/unirse) o con gremio.
public partial class GuildViewModel(IGuildService guildService, ISesionService sesionService) : ObservableObject
{
    [ObservableProperty] private bool estaOcupado;
    [ObservableProperty] private bool tieneGuild;
    [ObservableProperty] private bool cargaInicialTerminada;
    [ObservableProperty] private string nombreGuild = string.Empty;
    [ObservableProperty] private string rolGuild = string.Empty;

    // Formularios (estado sin gremio)
    [ObservableProperty] private string nuevoNombre = string.Empty;
    [ObservableProperty] private string nuevaDescripcion = string.Empty;
    [ObservableProperty] private string codigoInvitacion = string.Empty;

    // El backend solo devuelve el codigo al crear el gremio, por eso solo se puede mostrar aqui.
    [ObservableProperty] private string codigoGenerado = string.Empty;

    [ObservableProperty] private string mensajeError = string.Empty;

    public ObservableCollection<MiembroGuild> Miembros { get; } = new();

    public bool NoTieneGuild => CargaInicialTerminada && !TieneGuild;
    public bool MostrarContenido => CargaInicialTerminada;
    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool HayCodigoGenerado => !string.IsNullOrWhiteSpace(CodigoGenerado);
    public bool PuedeInteractuar => !EstaOcupado;
    public bool NoHayMiembros => Miembros.Count == 0;

    partial void OnTieneGuildChanged(bool value) => OnPropertyChanged(nameof(NoTieneGuild));
    partial void OnCargaInicialTerminadaChanged(bool value)
    {
        OnPropertyChanged(nameof(NoTieneGuild));
        OnPropertyChanged(nameof(MostrarContenido));
    }
    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));
    partial void OnCodigoGeneradoChanged(string value) => OnPropertyChanged(nameof(HayCodigoGenerado));
    partial void OnEstaOcupadoChanged(bool value) => OnPropertyChanged(nameof(PuedeInteractuar));

    [RelayCommand]
    public async Task CargarAsync()
    {
        if (EstaOcupado) return;
        EstaOcupado = true;
        MensajeError = string.Empty;

        try
        {
            var perfil = await sesionService.ObtenerPerfilAsync();
            if (!perfil.resultado || perfil.usuario is null)
            {
                MensajeError = MensajesError.Traducir(perfil.error, "No se pudo cargar tu información.");
                return;
            }

            TieneGuild = perfil.usuario.guidGuild.HasValue;
            NombreGuild = perfil.usuario.nombreGuild ?? string.Empty;
            RolGuild = perfil.usuario.rolGuild ?? string.Empty;

            Miembros.Clear();
            if (TieneGuild)
            {
                var miembros = await guildService.ObtenerMiembrosAsync();
                if (!miembros.resultado)
                {
                    MensajeError = MensajesError.Traducir(miembros.error, "No se pudieron cargar los miembros.");
                }
                else
                {
                    foreach (var miembro in miembros.miembros ?? new List<MiembroGuild>())
                    {
                        Miembros.Add(miembro);
                    }
                }
            }

            OnPropertyChanged(nameof(NoHayMiembros));
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            CargaInicialTerminada = true;
            EstaOcupado = false;
        }
    }

    [RelayCommand]
    private async Task CrearGuildAsync()
    {
        if (EstaOcupado) return;
        MensajeError = string.Empty;

        if (string.IsNullOrWhiteSpace(NuevoNombre))
        {
            MensajeError = "Ingresa el nombre del gremio.";
            return;
        }

        EstaOcupado = true;
        try
        {
            var res = await guildService.CrearAsync(NuevoNombre.Trim(), NuevaDescripcion?.Trim());
            if (!res.resultado || res.guild is null)
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudo crear el gremio.");
                return;
            }

            CodigoGenerado = res.guild.codigoInvitacion;
            NuevoNombre = string.Empty;
            NuevaDescripcion = string.Empty;
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
            return;
        }
        finally
        {
            EstaOcupado = false;
        }

        // Recarga ya con el gremio creado (el codigo generado se conserva en pantalla).
        await CargarAsync();
    }

    [RelayCommand]
    private async Task UnirseAsync()
    {
        if (EstaOcupado) return;
        MensajeError = string.Empty;

        string codigo = CodigoInvitacion.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(codigo))
        {
            MensajeError = "Ingresa el código de invitación.";
            return;
        }

        EstaOcupado = true;
        try
        {
            var res = await guildService.UnirseAsync(codigo);
            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudo unir al gremio.");
                return;
            }

            CodigoInvitacion = string.Empty;
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
            return;
        }
        finally
        {
            EstaOcupado = false;
        }

        await CargarAsync();
    }

    [RelayCommand]
    private async Task SalirAsync()
    {
        if (EstaOcupado) return;

        bool confirmar = await Shell.Current.DisplayAlertAsync(
            "Salir del gremio",
            $"¿Seguro que quieres salir de «{NombreGuild}»? Dejarás de ver sus listas compartidas.",
            "Sí, salir",
            "Cancelar");
        if (!confirmar) return;

        EstaOcupado = true;
        MensajeError = string.Empty;
        try
        {
            var res = await guildService.SalirAsync();
            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudo salir del gremio.");
                return;
            }

            CodigoGenerado = string.Empty;
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
            return;
        }
        finally
        {
            EstaOcupado = false;
        }

        await CargarAsync();
    }

    [RelayCommand]
    private async Task CopiarCodigoAsync()
    {
        if (!HayCodigoGenerado) return;

        await Clipboard.Default.SetTextAsync(CodigoGenerado);
        await Shell.Current.DisplayAlertAsync("Código copiado", "Compártelo con quien quieras invitar al gremio.", "OK");
    }

    [RelayCommand]
    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
