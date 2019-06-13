using PromoSeeker.Core;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PromoSeeker
{
    /// <summary>
    /// A converter that takes in a <see cref="ProductTrackingStatusType"/> and returns a vector resource for it.
    /// </summary>
    public class TrackingStatusToVectorConverter : BaseValueConverter<TrackingStatusToVectorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ProductTrackingStatusType)value)
            {
                case ProductTrackingStatusType.Idle: return Application.Current.Resources["Vector.TrackingIdle"];
                case ProductTrackingStatusType.Tracking: return Application.Current.Resources["Vector.Tracking"];
                case ProductTrackingStatusType.Updating: return Application.Current.Resources["Vector.TrackingUpdate"];
                case ProductTrackingStatusType.Error: return Application.Current.Resources["Vector.TrackingError"];
                case ProductTrackingStatusType.Disabled: return Application.Current.Resources["Vector.TrackingDisabled"];
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
