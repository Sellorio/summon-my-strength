using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SummonMyStrength.Views.Converters
{
    class EqualityToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var equal = value?.ToString() == parameter?.ToString();
            return equal ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
