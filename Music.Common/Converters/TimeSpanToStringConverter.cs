using System;
using System.Globalization;
using System.Windows.Data;

namespace Music.Common.Converters
{
    public sealed class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value < 3600 ? TimeSpan.FromSeconds((double)value).ToString(@"mm\:ss") : TimeSpan.FromSeconds((double)value).ToString(@"hh\:mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
