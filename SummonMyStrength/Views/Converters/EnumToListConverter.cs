using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SummonMyStrength.Views.Converters
{
    class EnumToListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var items = Enum.GetValues((Type)value);

            return
                parameter is string parameterString && parameterString == "Nullable"
                    ? items.Cast<object>().Append(null).ToList()
                    : (object)items;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
