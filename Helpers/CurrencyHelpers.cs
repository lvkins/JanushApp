using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PromoSeeker
{
    /// <summary>
    /// The currency symbols holder.
    /// </summary>
    public struct CurrencySymbol
    {
        /// <summary>
        /// The default symbol like $, €.
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// Extended, regional symbol like USD, EUR.
        /// </summary>
        public string ISO { get; set; }
    }

    /// <summary>
    /// A set of helpers methods for maintaining the currency.
    /// </summary>
    public static class CurrencyHelpers
    {
        #region Public Properties

        /// <summary>
        /// The dictionary of a cultures, with appropriate ISO currency symbol and currency symbol.
        /// </summary>
        public static Dictionary<string, CurrencySymbol> CurrencySymbols { get; }
            = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            .Select(_ => new { _.Name, Symbol = _.NumberFormat.CurrencySymbol, ISOSymbol = new RegionInfo(_.Name).ISOCurrencySymbol })
            .ToDictionary(_ => _.Name, _ => new CurrencySymbol { ISO = _.ISOSymbol, Default = _.Symbol });

        /// <summary>
        /// The group and decimal separators that are permitted in the price value.
        /// </summary>
        public static char[] PriceSeparators { get; } = new char[] { ',', '.', ' ' };

        #endregion

        #region Public Methods

        /// <summary>
        /// Replaces all ISO regional currency symbols to default currency symbols.
        /// </summary>
        /// <param name="input">The string where to replace the ISO symbols.</param>
        /// <returns></returns>
        public static string ISOSymbolToDefaultSymbol(string input)
        {
            // Iterate through the cultures...
            foreach (var symbol in CurrencySymbols.Values)
            {
                // If input contains the ISO symbol...
                if (input.Contains(symbol.ISO))
                {
                    // Replace any ISO currency symbol (whole word) with the default currency symbol
                    input = Regex.Replace(input, $@"\b{symbol.ISO}\b", symbol.Default);

                    // Break the loop
                    break;
                }
            }

            return input;
        }

        /// <summary>
        /// Finds the <see cref="CultureInfo"/> accordingly to the given price.
        /// </summary>
        /// <param name="input">The price.</param>
        /// <param name="outCulture">If culture was found, contains the readonly instance of the culture, otherwise, null.</param>
        /// <returns><see langword="true"/> if the culture could be found, otherwise <see langword="false"/>.</returns>
        public static bool FindCultureByPriceEx(string input, out CultureInfo outCulture)
        {
            // Replace any ISO symbol
            input = ISOSymbolToDefaultSymbol(input);

            // First, try the most obvious cultures
            // (because if we let the cultures loop do it, mostly likely it will return some different culture eg. for currencies like dollar).
            {
                CultureInfo culture = null;

                // Dollar
                if (input.Contains("$"))
                {
                    culture = CultureInfo.GetCultureInfo("en-US");
                }
                // Euro
                else if (input.Contains("€"))
                {
                    culture = CultureInfo.GetCultureInfo("de-DE");
                }
                // Pound
                else if (input.Contains("£"))
                {
                    culture = CultureInfo.GetCultureInfo("en-GB");
                }

                // If culture is set and we are able to parse the input using that culture...
                if (culture != null && decimal.TryParse(input, NumberStyles.Any, culture, out var value) && value > 0)
                {
                    outCulture = culture;
                    return true;
                }
            }

            // Iterate through the cultures
            foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                // If the currency symbol is found in the input (whole word - hence regex)...
                if (Regex.Match(input, $@"\b{Regex.Escape(culture.NumberFormat.CurrencySymbol)}\b", RegexOptions.IgnoreCase).Success)
                {
                    Console.WriteLine($"> {input}, {culture} / {culture.NumberFormat.CurrencySymbol}");
                    if (decimal.TryParse(input, NumberStyles.Any, culture, out var value))
                    {
                        // Return current culture
                        outCulture = culture;
                        return true;
                    }
                }
            }

            // The default value
            outCulture = default(CultureInfo);

            // Not found
            return false;
        }

        /// <summary>
        /// Fixes the price format (separators) accordingly to the given <paramref name="culture"/>.
        /// </summary>
        /// <param name="input">The price value to be formatted.</param>
        /// <param name="culture">The culture to format the input accordingly to.</param>
        /// <returns>Returns a formatted price value.</returns>
        public static string FixPriceFormat(string input, CultureInfo culture)
        {
            // Trim any white-spaces characters
            input = input.Trim();

            // Convert the price to char array
            var priceArray = input.ToCharArray();

            // The first digit index
            var firstDigitId = Array.FindIndex(priceArray, _ => char.IsDigit(_));

            // The last digit index
            var lastDigitId = Array.FindLastIndex(priceArray, _ => char.IsDigit(_));

            // If we have the digit position...
            if (firstDigitId != -1)
            {
                // Extract only price value
                var priceValue = input.Substring(firstDigitId, (lastDigitId + 1) - firstDigitId).Trim();

                // Get decimal point position
                var decimalIndex = priceValue.LastIndexOfAny(PriceSeparators);

                // Split the price by separators and remove empty entries
                var pieces = priceValue.Split(PriceSeparators, StringSplitOptions.RemoveEmptyEntries);

                // Join groups with group separator but skip the decimal
                var newPriceValue = string.Join(culture.NumberFormat.CurrencyGroupSeparator, pieces, 0, decimalIndex != -1 ? pieces.Length - 1 : pieces.Length);

                // If decimal point is available...
                if (decimalIndex != -1)
                {
                    // Append decimal separator along with the remaining decimal value
                    newPriceValue += culture.NumberFormat.CurrencyDecimalSeparator + pieces.Last();
                }

                // If the price value was changed...
                if (priceValue != newPriceValue)
                {
                    // Remove old price and insert the new one
                    input = input.Remove(firstDigitId, (lastDigitId + 1) - firstDigitId).Insert(firstDigitId, priceValue);
                }
            }

            return input;
        }

        /// <summary>
        /// Roughly extracts the price characters from the input string and attempts to format it 
        /// accordingly to the culture info in order to successfully convert it to a decimal type.
        /// </summary>
        /// <param name="input">The input string to be converted to decimal value.</param>
        /// <param name="result">When this method returns, contains the <see cref="decimal"/> number
        /// value of the <paramref name="input"/> that is acquired through the formatting process, 
        /// if the conversion succeeded, or is zero if the conversion failed.</param>
        /// <param name="culture">The culture info to be used to format decimal numbers. If not specified, the <see cref="CultureInfo.CurrentCulture"/> will be used.</param>
        /// <returns><see langword="true"/> if <paramref name="input"/> was converted successfully, otherwise <see langword="false"/>.</returns>
        public static bool ExtractPrice(string input, out decimal result, CultureInfo culture)
        {
            // If bad input...
            if (string.IsNullOrWhiteSpace(input))
            {
                // Return zero
                result = 0;
                return false;
            }

            // If the input can be parsed as decimal at this point...
            if (decimal.TryParse(input, NumberStyles.Any, culture, out var tmpDec))
            {
                // Format the input string
                input = tmpDec.ToString("N", culture);
            }

            // Extract only the price characters (positive) from string
            // Examples:
            //  abcdefgh1234wegnweo => 1234
            //  (1234.56)           => 1234.56
            //  4321,30             => 4321,30
            //  -159753             => 159753
            input = string.Concat(input.Where(_ => char.IsDigit(_) || PriceSeparators.Contains(_)));

            // Trim the result to prevent any leading or trailing whitespaces that could've been taken as a separator
            input = input.Trim();

            // Fix the price format accordingly to the culture for proper decimal conversion

            // Get decimal point position
            var decimalIndex = input.LastIndexOfAny(PriceSeparators);

            // If decimal point is available and there is more than one character after the decimal separator...
            /*if (decimalIndex != -1 && input.Length > decimalIndex + 3)
            {
                // Truncate trailing characters for decimal places compatibility (so that 139,8 == 139,80)
                input = input.Substring(0, decimalIndex + 3);
            }*/

            // Split the price by separators and remove empty entries
            var pieces = input.Split(PriceSeparators, StringSplitOptions.RemoveEmptyEntries);

            // If we have no pieces...
            if (!pieces.Any())
            {
                // We have a bad input, return zero
                result = 0;
                return false;
            }

            // Join groups with group separator but skip the decimal
            input = string.Join(culture.NumberFormat.CurrencyGroupSeparator, pieces, 0, decimalIndex != -1 ? pieces.Length - 1 : pieces.Length);

            // If decimal point is available...
            if (decimalIndex != -1)
            {
                // Append decimal separator along with the remaining decimal value
                input += culture.NumberFormat.CurrencyDecimalSeparator + pieces.Last();
            }

            // Attempt to convert and return the result
            decimal.TryParse(input, NumberStyles.Any, culture, out result);

            // Assume zero is not a price
            return result > 0;
        }

        /// <summary>
        /// Gets any currency symbol from the given <paramref name="price"/> value.
        /// </summary>
        /// <param name="price">The value to search for the currency symbol.</param>
        /// <param name="result">If succeeded, contains the currency symbol found in the given price, otherwise null.</param>
        /// <returns><see langword="true"/> if the currency symbol was found in the <paramref name="price"/> value, otherwise, <see langword="false"/>.</returns>
        public static bool FindCurrencySymbol(string price, out string result)
        {
            // Basic idea is to remove anything between the first and last digit in the string (so, basically the price, including any decimal and group separators).
            // The leftover is our currency symbol.

            // Convert the price to char array
            var priceArray = price.ToCharArray();

            // The first digit index
            var firstDigitId = Array.FindIndex(priceArray, _ => char.IsDigit(_));

            // The last digit index
            var lastDigitId = Array.FindLastIndex(priceArray, _ => char.IsDigit(_));

            // If first digit was found...
            if (firstDigitId != -1)
            {
                // Remove anything between first & last digit, trim any white-space characters
                result = price.Remove(firstDigitId, (lastDigitId - firstDigitId) + 1).Trim();

                // We have the symbol if the result is not empty
                return !string.IsNullOrEmpty(result);
            }

            // Not found
            result = default(string);

            return false;
        }

        /// <summary>
        /// Attempts to find the <see cref="CultureInfo"/> which currency symbol is equal to the specified <paramref name="symbol"/>.
        /// </summary>
        /// <param name="symbol">The currency symbol by which to find the culture.</param>
        /// <param name="result">If succeeded, contains the <see cref="CultureInfo"/> info found, otherwise, null.</param>
        /// <returns><see langword="true"/> if culture was found, otherwise, <see langword="false"/>.</returns>
        public static bool FindCultureByCurrencySymbol(string symbol, out CultureInfo result)
        {
            // The most obvious first
            // (because if we let the cultures loop do it, mostly likely it will return some different culture eg. for currencies like dollar).

            // Dollar
            if (symbol == "$")
            {
                result = CultureInfo.GetCultureInfo("en-US");
                return true;
            }

            // Euro
            if (symbol == "€")
            {
                result = CultureInfo.GetCultureInfo("de-DE");
                return true;
            }

            // Pound
            if (symbol == "£")
            {
                result = CultureInfo.GetCultureInfo("en-GB");
                return true;
            }

            // Iterate through the cultures
            foreach (var cultureSymbol in CurrencySymbols)
            {
                // If symbol equals either ISO or default currency symbol...
                if (symbol.Equals(cultureSymbol.Value.Default, StringComparison.OrdinalIgnoreCase) ||
                    symbol.Equals(cultureSymbol.Value.ISO, StringComparison.OrdinalIgnoreCase))
                {
                    // Return the culture info
                    result = CultureInfo.GetCultureInfo(cultureSymbol.Key);
                    return true;
                }
            }

            // Not found, return default
            result = null;
            return false;
        }

        /// <summary>
        /// Attempts to find the <see cref="CultureInfo"/> using which the <paramref name="price"/> value was formatted, by parsing it's currency symbol.
        /// </summary>
        /// <param name="price">The full price value that includes the currency symbol.</param>
        /// <param name="result">If succeeded, contains the <see cref="CultureInfo"/> info found, otherwise, null.</param>
        /// <returns><see langword="true"/> if culture was found, otherwise, <see langword="false"/>.</returns>
        public static bool FindCultureByPrice(string price, out CultureInfo result)
        {
            // Find the price currency symbol
            if (!FindCurrencySymbol(price, out var symbol))
            {
                // No symbol was found. Return with the default value.
                result = null;
                return false;
            }

            // Get culture by found currency symbol and return.
            return FindCultureByCurrencySymbol(symbol, out result);
        }

        #endregion

    }
}
