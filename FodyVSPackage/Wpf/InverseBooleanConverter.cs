using System;
using System.Globalization;
using System.Windows.Data;

namespace Wpf
{
    [ValueConversion(typeof (bool), typeof (bool))]
    public class InverseBooleanConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}