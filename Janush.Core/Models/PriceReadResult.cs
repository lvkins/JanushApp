﻿namespace Janush.Core
{
    /// <summary>
    /// The <see cref="Product"/> price read result object. Contains the informations about the parsed price.
    /// </summary>
    public class PriceReadResult
    {
        #region Public Properties

        /// <summary>
        /// The original, unformatted price string value as found during parsing.
        /// </summary>
        public string Original { get; set; }

        /// <summary>
        /// The formatted price string value.
        /// </summary>
        public string Raw { get; set; }

        /// <summary>
        /// The currency symbol, if any, that was found in the <see cref="Original"/> price value.
        /// </summary>
        public string CurrencySymbol { get; set; }

        /// <summary>
        /// The <see cref="Raw"/> value converted to a decimal value.
        /// </summary>
        public decimal Decimal { get; set; }

        /// <summary>
        /// Identifies whether the price value is considered as valid.
        /// </summary>
        public bool Valid { get; set; }

        #endregion
    }
}
