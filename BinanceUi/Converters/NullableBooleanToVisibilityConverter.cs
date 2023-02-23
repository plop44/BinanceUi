using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BinanceUi.Converters;

public class NullableBooleanToVisibilityConverter : IValueConverter
{
    public bool Inverse { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return Visibility.Collapsed;

        var boolValue = (bool)value;

        return boolValue ^ Inverse ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}