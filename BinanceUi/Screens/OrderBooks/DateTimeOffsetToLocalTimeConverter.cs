using System;
using System.Globalization;
using System.Windows.Data;

namespace BinanceUi.Screens.OrderBooks;

public class DateTimeOffsetToLocalTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.LocalDateTime;
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}