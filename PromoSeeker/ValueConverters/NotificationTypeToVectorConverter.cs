using PromoSeeker.Core;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PromoSeeker
{
    /// <summary>
    /// A converter that takes in a <see cref="NotificationType"/> and returns a vector resource for it.
    /// </summary>
    public class NotificationTypeToVectorConverter : BaseValueConverter<NotificationTypeToVectorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((NotificationType)value)
            {
                case NotificationType.PriceDown: return Application.Current.Resources["Vector.PriceDown"];
                case NotificationType.PriceUp: return Application.Current.Resources["Vector.PriceUp"];
                case NotificationType.NameChange: return Application.Current.Resources["Vector.TypeName"];
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
