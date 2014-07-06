using System;
using System.Globalization;
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace VideaCesky
{
    public sealed class IntToBoolConverter : IValueConverter
    {
        private enum Operator
        {
            Equal,
            NotEqual,
            Greater,
            GreaterOrEqual,
            Lower,
            LowerOrEqual
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string numberParamString = (string)parameter;

            Operator op = Operator.Equal;

            if (numberParamString.StartsWith("!="))
            {
                op = Operator.NotEqual;
                numberParamString = numberParamString.Substring(2);
            }
            else if (numberParamString.StartsWith(">"))
            {
                op = Operator.Greater;
                numberParamString = numberParamString.Substring(1);
            }
            else if (numberParamString.StartsWith(">="))
            {
                op = Operator.GreaterOrEqual;
                numberParamString = numberParamString.Substring(2);
            }
            else if (numberParamString.StartsWith("<"))
            {
                op = Operator.Lower;
                numberParamString = numberParamString.Substring(1);
            }
            else if (numberParamString.StartsWith("<="))
            {
                op = Operator.LowerOrEqual;
                numberParamString = numberParamString.Substring(2);
            }

            int number = (int)value;
            int numberParam = System.Convert.ToInt32(numberParamString);

            switch (op)
            {
            case Operator.Greater: return (number > numberParam);
            case Operator.GreaterOrEqual: return (number >= numberParam);
            case Operator.Lower: return (number < numberParam);
            case Operator.LowerOrEqual: return (number <= numberParam);
            case Operator.NotEqual: return (number != numberParam);
            case Operator.Equal:
            default: return (number == numberParam);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
