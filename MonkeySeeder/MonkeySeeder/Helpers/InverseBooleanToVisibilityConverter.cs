using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace MonkeySeeder.Helpers
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolConv = new BooleanToVisibilityConverter();
            return boolConv.Convert(!(bool)value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolConv = new BooleanToVisibilityConverter();
            return boolConv.ConvertBack(!(bool)value, targetType, parameter, culture);
        }
    }
}