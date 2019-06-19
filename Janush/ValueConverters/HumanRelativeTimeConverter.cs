using System;
using System.Globalization;
using Janush.Core;

namespace Janush
{
    /// <summary>
    /// A converter that takes in a <see cref="DateTime"/> value and returns a human relative time string comparing to the current date.
    /// </summary>
    public class HumanRelativeTimeConverter : BaseValueConverter<HumanRelativeTimeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => ((DateTime)value).ToHumanRelative();

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
