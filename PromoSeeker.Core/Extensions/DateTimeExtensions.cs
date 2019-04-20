using System;

namespace PromoSeeker.Core
{
    /// <summary>
    /// The extension methods for the <see cref="DateTime"/> type object.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Calculates the relative time elapsed from this date up to the current date.
        /// </summary>
        /// <param name="input">The past date.</param>
        /// <returns>The 'time ago' type string this date was.</returns>
        public static string ToHumanRelative(this DateTime input)
        {
            // TODO: Localize me

            // Get elapsed time
            var elapsed = DateTime.Now.Subtract(input);

            // Seconds
            if (elapsed.TotalSeconds < 0)
            {
                // We don't handle future values
                return string.Empty;
            }
            else if (elapsed.TotalSeconds < 10)
            {
                return "just now";
            }
            else if (elapsed.TotalSeconds < 60)
            {
                return $"{Convert.ToInt32(elapsed.TotalSeconds)} seconds ago";
            }
            // Minutes
            else if ((int)elapsed.TotalMinutes == 1)
            {
                return "a minute ago";
            }
            else if (elapsed.TotalHours < 1)
            {
                return $"{Convert.ToInt32(elapsed.TotalMinutes)} minutes ago";
            }
            // Hours
            else if ((int)elapsed.TotalHours == 1)
            {
                return "an hour ago";
            }
            else if (elapsed.TotalHours < 24)
            {
                return $"{Convert.ToInt32(elapsed.TotalHours)} hours ago";
            }
            // Days
            else if ((int)elapsed.TotalDays == 1)
            {
                return "yesterday";
            }
            else if (elapsed.TotalDays < 7)
            {
                return $"{Convert.ToInt32(elapsed.TotalDays)} days ago";
            }
            // Weeks
            else if ((int)elapsed.TotalDays == 7)
            {
                return "a week ago";
            }
            else if (elapsed.TotalDays < DateTime.DaysInMonth(input.Year, input.Month))
            {
                return $"{Convert.ToInt32(elapsed.TotalDays / 7)} weeks ago";
            }
            // Months
            else if ((int)elapsed.TotalDays == DateTime.DaysInMonth(input.Year, input.Month))
            {
                return "a month ago";
            }
            else if (elapsed.TotalDays < (DateTime.IsLeapYear(input.Year) ? 366 : 365))
            {
                var daysInYear = (DateTime.IsLeapYear(input.Year) ? 366 : 365);
                return $"{Convert.ToInt32(elapsed.TotalDays / (daysInYear / 12))} months ago";
            }
            // Years
            else if ((int)elapsed.TotalDays == (DateTime.IsLeapYear(input.Year) ? 366 : 365))
            {
                return "a year ago";
            }

            // Total years
            return $"{Convert.ToInt32(elapsed.TotalDays / 365.2425)} years ago";
        }
    }
}
