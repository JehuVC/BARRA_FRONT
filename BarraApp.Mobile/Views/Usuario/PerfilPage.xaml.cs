using System.ComponentModel;
using BarraApp.Mobile.ViewModels.Usuario;

namespace BarraApp.Mobile.Views.Usuario;

public partial class PerfilPage : ContentPage
{
    private readonly PerfilViewModel _viewModel;

    public PerfilPage(PerfilViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarPerfilCommand.ExecuteAsync(null);
    }

    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PerfilViewModel.HasSuccess) && _viewModel.HasSuccess)
        {
            // Animación suave de entrada para la notificación de éxito
            NotificacionExito.Opacity = 0;
            NotificacionExito.TranslationY = -15;
            await Task.WhenAll(
                NotificacionExito.FadeTo(1, 300, Easing.CubicOut),
                NotificacionExito.TranslateTo(0, 0, 300, Easing.SpringOut)
            );
        }
        else if (e.PropertyName == nameof(PerfilViewModel.HasError) && _viewModel.HasError)
        {
            // Animación para el mensaje de error
            NotificacionError.Opacity = 0;
            NotificacionError.TranslationY = -15;
            await Task.WhenAll(
                NotificacionError.FadeTo(1, 300, Easing.CubicOut),
                NotificacionError.TranslateTo(0, 0, 300, Easing.SpringOut)
            );
        }
    }
}