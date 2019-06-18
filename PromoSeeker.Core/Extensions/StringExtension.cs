using System;

namespace PromoSeeker.Core
{
    /// <summary>
    /// The extension methods for the <see cref="string"/>.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Enhanced version of <see cref="string.Contains(string)"/> to support comparisons.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="comparsionType"></param>
        /// <returns></returns>
        public static bool ContainsEx(this string source, string value, StringComparison comparsionType = StringComparison.OrdinalIgnoreCase)
            => source?.IndexOf(value, comparsionType) >= 0;

        /// <summary>
        /// Returns a value indicating whether any specified substring in the array occurs within this string.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="values">The array of values to seek.</param>
        /// <param name="comparsionType">The string comparison method.</param>
        /// <returns><see langword="true"/> if any value from the <paramref name="values"/> parameter occurs within this string, otherwise <see langword="false"/>.</returns>
        public static bool ContainsAny(this string source, string[] values, StringComparison comparsionType = StringComparison.OrdinalIgnoreCase)
        {
            foreach (var value in values)
            {
                if (source.ContainsEx(value, comparsionType))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
