using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace TDT4900_MasterThesis.View.Converter;

public class EnumToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
            return AvaloniaProperty.UnsetValue;

        return value.ToString() == parameter.ToString();
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotImplementedException();
    }
}
