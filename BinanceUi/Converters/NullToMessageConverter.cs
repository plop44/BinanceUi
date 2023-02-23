using System;
using System.Globalization;
using System.Windows.Data;

namespace BinanceUi.Converters;

public class NullToMessageConverter : IValueConverter
{
    public string NullValue { get; set; } = "NullValue";
    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        return value ?? NullValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}