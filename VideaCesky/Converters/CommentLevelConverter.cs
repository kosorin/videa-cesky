using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace VideaCesky.Converters
{
    public class CommentLevelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is int)
            {
                return new Thickness((int)value * 40, 0, 0, 0);
            }
            else
            {
                return new Thickness(0);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
