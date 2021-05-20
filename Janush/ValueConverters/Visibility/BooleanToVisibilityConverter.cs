// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Janush
{
    /// <summary>
    /// A converter that takes in a boolean value and returns a <see cref="Visibility"/> type.
    /// </summary>
    internal class BooleanToVisibilityConverter : BaseValueConverter<BooleanToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Handle integers as booleans, just to avoid another ZeroToVisibilityConverter...
            // (null/0 = false, anything else = 1)
            if (value is int)
            {
                value = System.Convert.ToBoolean(value);
            }

            if (parameter == null)
                return (bool)value ? Visibility.Collapsed : Visibility.Visible;
            else
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
