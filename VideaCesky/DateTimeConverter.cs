using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace VideaCesky
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime)
            {
                return string.Format(parameter as string ?? "{0:f}", (DateTime)value);
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
