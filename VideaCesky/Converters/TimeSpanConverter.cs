using System;
using Windows.UI.Xaml.Data;

namespace VideaCesky.Converters
{
    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            TimeSpan timeSpan = (TimeSpan)value;
            return timeSpan.ToString(timeSpan.Hours > 0 ? "hh':'mm':'ss" : "mm':'ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
