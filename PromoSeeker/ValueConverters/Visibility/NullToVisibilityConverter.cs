using System;
using System.Globalization;
using System.Windows;

namespace PromoSeeker
{
    /// <summary>
    /// A converter that takes in a value and returns a <see cref="Visibility"/> type depending on whether the value is null or not.
    /// </summary>
    internal class NullToVisibilityConverter : BaseValueConverter<NullToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return value == null ? Visibility.Collapsed : Visibility.Visible;
            else
                return value == null ? Visibility.Visible : Visibility.Hidden;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
