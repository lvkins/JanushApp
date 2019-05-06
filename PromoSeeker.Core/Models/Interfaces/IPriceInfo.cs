namespace PromoSeeker.Core
{
    /// <summary>
    /// Holds all the relevant informations about the product price and it's origin.
    /// </summary>
    public interface IPriceInfo
    {
        #region Properties
        /// <summary>
        /// The type of price origin.
        /// </summary>
        PriceSourceType Source { get; set; }

        /// <summary>
        /// The node attribute name where the price is defined.
        /// </summary>
        string AttributeName { get; set; }

        /// <summary>
        /// The selector used to locate the price origin node in the document.
        /// </summary>
        string PriceXPathOrSelector { get; set; }

        /// <summary>
        /// The price value.
        /// </summary>
        decimal Value { get; }

        /// <summary>
        /// The <see cref="Value"/> decimal value converted to a string that represent a currency amount.
        /// </summary>
        string CurrencyAmount { get; }

        #endregion
    }
}
