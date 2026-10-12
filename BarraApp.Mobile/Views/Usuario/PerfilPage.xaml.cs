using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using BarraApp.Mobile.ViewModels.Usuario;

namespace BarraApp.Mobile.Views.Usuario;

public partial class PerfilPage : ContentPage
{
    private readonly PerfilViewModel _viewModel;
    private CancellationTokenSource? _toastCts;

    public PerfilPage(PerfilViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // 1. Estado inicial para la animación de entrada suave
        TarjetaUsuario.Opacity = 0;
        TarjetaUsuario.TranslationY = -20;

        TarjetaFormulario.Opacity = 0;
        TarjetaFormulario.TranslationY = 25;

        BotonCerrarSesion.Opacity = 0;
        BotonCerrarSesion.TranslationY = 15;

        // 2. Disparar carga de datos
        _ = _viewModel.CargarPerfilCommand.ExecuteAsync(null);

        // 3. Coreografía de animación de entrada fluida
        await Task.WhenAll(
            TarjetaUsuario.FadeTo(1, 350, Easing.CubicOut),
            TarjetaUsuario.TranslateTo(0, 0, 400, Easing.SpringOut)
        );

        await Task.WhenAll(
            TarjetaFormulario.FadeTo(1, 350, Easing.CubicOut),
            TarjetaFormulario.TranslateTo(0, 0, 400, Easing.CubicOut)
        );

        await Task.WhenAll(
            BotonCerrarSesion.FadeTo(1, 300, Easing.CubicOut),
            BotonCerrarSesion.TranslateTo(0, 0, 300, Easing.CubicOut)
        );
    }

    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PerfilViewModel.HasSuccess) && _viewModel.HasSuccess)
        {
            await MostrarToastAnimadoAsync(NotificacionExito);
        }
        else if (e.PropertyName == nameof(PerfilViewModel.HasError) && _viewModel.HasError)
        {
            await MostrarToastAnimadoAsync(NotificacionError);
        }
    }

    private async Task MostrarToastAnimadoAsync(VisualElement toastElement)
    {
        _toastCts?.Cancel();
        _toastCts = new CancellationTokenSource();
        var token = _toastCts.Token;

        // 1. Deslizar hacia abajo desde la parte superior (efecto flotante)
        toastElement.TranslationY = -50;
        toastElement.Opacity = 0;
        await Task.WhenAll(
            toastElement.FadeTo(1, 260, Easing.CubicOut),
            toastElement.TranslateTo(0, 0, 320, Easing.SpringOut)
        );

        // 2. Esperar 3.5 segundos antes de ocultarse automáticamente
        try
        {
            await Task.Delay(3500, token);
            if (!token.IsCancellationRequested)
            {
                // 3. Desvanecer y regresar hacia arriba suavemente
                await Task.WhenAll(
                    toastElement.FadeTo(0, 250, Easing.CubicIn),
                    toastElement.TranslateTo(0, -50, 250, Easing.CubicIn)
                );
                _viewModel.CerrarNotificacionCommand.Execute(null);
            }
        }
        catch (TaskCanceledException)
        {
            // Ocurre de forma normal si el usuario cierra el toast antes con la cruz
        }
    }
}