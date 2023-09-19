using System;
using System.Globalization;
using System.Windows.Data;

namespace Music.WPF.Converters
{
    public sealed class TrackCountToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return "0 Tracks";

            return ((int)value == 1) ? $"1 Track" : $"{value} Tracks";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
