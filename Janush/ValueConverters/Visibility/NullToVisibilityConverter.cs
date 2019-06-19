using System;
using System.Globalization;
using System.Windows;

namespace Janush
{
    /// <summary>
    /// A converter that takes in a value and returns a <see cref="Visibility"/> type depending on whether the value is null or not.
    /// </summary>
    internal class NullToVisibilityConverter : BaseValueConverter<NullToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Also handle string case
            var isNull = value is string s
                ? string.IsNullOrEmpty(s)
                : value == null;

            if (parameter == null)
                return isNull ? Visibility.Collapsed : Visibility.Visible;
            else
                return isNull ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
