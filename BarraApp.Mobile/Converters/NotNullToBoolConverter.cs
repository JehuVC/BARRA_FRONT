using System.Globalization;

namespace BarraApp.Mobile.Converters;

// true cuando el valor enlazado no es null (ej. una lista con GuidGuild es "compartida").
public class NotNullToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is not null;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
