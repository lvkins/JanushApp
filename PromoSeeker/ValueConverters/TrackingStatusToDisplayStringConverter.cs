using PromoSeeker.Core;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PromoSeeker
{
    /// <summary>
    /// A converter that takes in a <see cref="ProductTrackingStatusType"/> and returns a localized display string of it.
    /// </summary>
    public class TrackingStatusToDisplayStringConverter : BaseValueConverter<TrackingStatusToDisplayStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((ProductTrackingStatusType)value).ToDisplayString();

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
