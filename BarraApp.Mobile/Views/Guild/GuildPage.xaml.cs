using BarraApp.Mobile.ViewModels.Guild;

namespace BarraApp.Mobile.Views.Guild;

public partial class GuildPage : ContentPage
{
    private readonly GuildViewModel _viewModel;

    public GuildPage(GuildViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarCommand.ExecuteAsync(null);
    }
}
