using System.Collections.ObjectModel;
using BarraApp.Mobile.Services.Api;
using BarraApp.Mobile.Services.ListaCompra;
using BarraApp.Mobile.Views.ListaCompra;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ListaCompraEntidad = Core.Entidades.Enti_Clases.ListaCompra;

namespace BarraApp.Mobile.ViewModels.ListaCompra;

// FE-S9-06 (ver listas), FE-S9-07 (crear), FE-S9-08 (renombrar) y FE-S9-09 (eliminar).
public partial class ListasViewModel(IListaCompraService listaService) : ObservableObject
{
    [ObservableProperty] private bool estaOcupado;
    [ObservableProperty] private bool cargaInicialTerminada;
    [ObservableProperty] private string mensajeError = string.Empty;

    public ObservableCollection<ListaCompraEntidad> Listas { get; } = new();

    public bool HasError => !string.IsNullOrWhiteSpace(MensajeError);
    public bool PuedeInteractuar => !EstaOcupado;
    public bool NoHayListas => CargaInicialTerminada && Listas.Count == 0;

    partial void OnMensajeErrorChanged(string value) => OnPropertyChanged(nameof(HasError));
    partial void OnEstaOcupadoChanged(bool value) => OnPropertyChanged(nameof(PuedeInteractuar));
    partial void OnCargaInicialTerminadaChanged(bool value) => OnPropertyChanged(nameof(NoHayListas));

    [RelayCommand]
    public async Task CargarAsync()
    {
        if (EstaOcupado) return;
        EstaOcupado = true;
        MensajeError = string.Empty;

        try
        {
            var res = await listaService.ObtenerListasAsync();
            if (!res.resultado)
            {
                MensajeError = MensajesError.Traducir(res.error, "No se pudieron cargar tus listas.");
                return;
            }

            Listas.Clear();
            foreach (var lista in (res.Listas ?? new List<ListaCompraEntidad>()).OrderByDescending(l => l.FechaModificacion))
            {
                Listas.Add(lista);
            }
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            CargaInicialTerminada = true;
            OnPropertyChanged(nameof(NoHayListas));
            EstaOcupado = false;
        }
    }

    [RelayCommand]
    private async Task CrearAsync()
    {
        if (EstaOcupado) return;

        string? nombre = await Shell.Current.DisplayPromptAsync(
            "Nueva lista", "Nombre de la lista", "Crear", "Cancelar",
            placeholder: "Ej. Compras de la semana", maxLength: 100);

        if (string.IsNullOrWhiteSpace(nombre)) return;

        if (await EjecutarAsync(async () =>
            {
                var res = await listaService.CrearAsync(nombre.Trim());
                return res.resultado ? null : MensajesError.Traducir(res.error, "No se pudo crear la lista.");
            }))
        {
            await CargarAsync();
        }
    }

    [RelayCommand]
    private async Task RenombrarAsync(ListaCompraEntidad lista)
    {
        if (EstaOcupado || lista is null) return;

        string? nombre = await Shell.Current.DisplayPromptAsync(
            "Renombrar lista", "Nuevo nombre", "Guardar", "Cancelar",
            initialValue: lista.Nombre, maxLength: 100);

        if (string.IsNullOrWhiteSpace(nombre) || nombre.Trim() == lista.Nombre) return;

        if (await EjecutarAsync(async () =>
            {
                var res = await listaService.ActualizarAsync(lista.GuidLista, nombre.Trim(), lista.FechaModificacion);
                return res.resultado ? null : MensajesError.Traducir(res.error, "No se pudo renombrar la lista.");
            }))
        {
            await CargarAsync();
        }
    }

    [RelayCommand]
    private async Task EliminarAsync(ListaCompraEntidad lista)
    {
        if (EstaOcupado || lista is null) return;

        bool confirmar = await Shell.Current.DisplayAlertAsync(
            "Eliminar lista",
            $"¿Eliminar «{lista.Nombre}»? Esta acción no se puede deshacer.",
            "Sí, eliminar",
            "Cancelar");
        if (!confirmar) return;

        if (await EjecutarAsync(async () =>
            {
                var res = await listaService.EliminarAsync(lista.GuidLista);
                return res.resultado ? null : MensajesError.Traducir(res.error, "No se pudo eliminar la lista.");
            }))
        {
            await CargarAsync();
        }
    }

    [RelayCommand]
    private async Task AbrirAsync(ListaCompraEntidad lista)
    {
        if (lista is null) return;

        await Shell.Current.GoToAsync(
            $"{nameof(ListaDetallePage)}?guidLista={lista.GuidLista}&nombre={Uri.EscapeDataString(lista.Nombre)}");
    }

    [RelayCommand]
    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    // Corre una accion que devuelve el mensaje de error (o null si salio bien). true = salio bien.
    private async Task<bool> EjecutarAsync(Func<Task<string?>> accion)
    {
        EstaOcupado = true;
        MensajeError = string.Empty;
        try
        {
            string? error = await accion();
            if (error is not null)
            {
                MensajeError = error;
                return false;
            }

            return true;
        }
        catch (ApiException ex)
        {
            MensajeError = ex.Message;
            return false;
        }
        finally
        {
            EstaOcupado = false;
        }
    }
}
