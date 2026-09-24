using System.Windows.Input;

namespace BarraApp.Mobile.Controls;

public partial class FloatingEntry : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(FloatingEntry), string.Empty, BindingMode.TwoWay);

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(FloatingEntry), string.Empty);

    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(
        nameof(IsPassword), typeof(bool), typeof(FloatingEntry), false);

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
        nameof(Keyboard), typeof(Keyboard), typeof(FloatingEntry), Keyboard.Default);

    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
        nameof(BorderColor), typeof(Brush), typeof(FloatingEntry), new SolidColorBrush(Color.FromArgb("#374151")));

    public static readonly BindableProperty HasInfoProperty = BindableProperty.Create(
        nameof(HasInfo), typeof(bool), typeof(FloatingEntry), false);

    public static readonly BindableProperty InfoCommandProperty = BindableProperty.Create(
        nameof(InfoCommand), typeof(ICommand), typeof(FloatingEntry));

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public bool IsPassword { get => (bool)GetValue(IsPasswordProperty); set => SetValue(IsPasswordProperty, value); }
    public Keyboard Keyboard { get => (Keyboard)GetValue(KeyboardProperty); set => SetValue(KeyboardProperty, value); }
    public Brush BorderColor { get => (Brush)GetValue(BorderColorProperty); set => SetValue(BorderColorProperty, value); }
    public bool HasInfo { get => (bool)GetValue(HasInfoProperty); set => SetValue(HasInfoProperty, value); }
    public ICommand InfoCommand { get => (ICommand)GetValue(InfoCommandProperty); set => SetValue(InfoCommandProperty, value); }

    public FloatingEntry()
    {
        InitializeComponent();
    }

    private async void OnEntryFocused(object sender, FocusEventArgs e) => await AnimatePlaceholder(true);

    private async void OnEntryUnfocused(object sender, FocusEventArgs e)
    {
        if (string.IsNullOrEmpty(Text)) await AnimatePlaceholder(false);
    }

    private async void OnEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.NewTextValue) && !MainEntry.IsFocused) await AnimatePlaceholder(true);
    }

    private async Task AnimatePlaceholder(bool isFocusedOrHasText)
    {
        if (isFocusedOrHasText)
        {
            PlaceholderLabel.TextColor = Color.FromArgb("#FFD700");
            PlaceholderLabel.FontSize = 11;
            // Sube -14 para quedar perfectamente alineado arriba
            await PlaceholderLabel.TranslateTo(0, -14, 150, Easing.CubicOut);
        }
        else
        {
            PlaceholderLabel.FontSize = 14;
            PlaceholderLabel.TextColor = Color.FromArgb("#9CA3AF");
            await PlaceholderLabel.TranslateTo(0, 0, 150, Easing.CubicIn);
        }
    }
}