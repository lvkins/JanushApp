// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Windows.Data;
using Janush.Core;

namespace Janush
{
    /// <summary>
    /// A converter that takes in a <see cref="ProductTrackingStatusType"/> and the error value, if any and readable product status.
    /// </summary>
    public class TrackingStatusToDisplayStringConverter : BaseMultiValueConverter<TrackingStatusToDisplayStringConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (ProductTrackingStatusType)values[1];

            if (values[0] is string error && !string.IsNullOrWhiteSpace(error))
            {
                return string.Format(status.ToDisplayString(), error);
            }

            return status.ToDisplayString();
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
