namespace PromoSeeker
{
    /// <summary>
    /// The price object that is created during parsing a web document. Contains the informations about the parsed price.
    /// </summary>
    public class PriceValue
    {
        #region Public Properties

        /// <summary>
        /// The original, unformatted price string value.
        /// </summary>
        public string Original { get; set; }

        /// <summary>
        /// The formatted price string value.
        /// </summary>
        public string Raw { get; set; }

        /// <summary>
        /// The detected currency symbol of the price.
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
