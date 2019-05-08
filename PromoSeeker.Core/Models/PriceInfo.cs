using System;

namespace PromoSeeker.Core
{
    /// <summary>
    /// The types of the price origin.
    /// </summary>
    public enum PriceSourceType
    {
        /// <summary>
        /// A price that is coming from an attribute.
        /// </summary>
        PriceSourceAttribute,

        /// <summary>
        /// A price that is coming from the Javascript parsing.
        /// </summary>
        PriceSourceJavascript,

        /// <summary>
        /// A price that is coming from the raw node text.
        /// </summary>
        PriceSourceNode,
    }

    /// <summary>
    /// A class that holds all the relevant informations about the product price and it's origin.
    /// </summary>
    public class PriceInfo : IPriceInfo
    {
        #region Public Properties

        /// <summary>
        /// The type of price origin.
        /// </summary>
        public PriceSourceType Source { get; set; }

        /// <summary>
        /// The node attribute name where the price is defined.
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// The XPath query or a selector used to locate the price origin node in the document.
        /// </summary>
        public string PriceXPathOrSelector { get; set; }

        /// <summary>
        /// The price value.
        /// </summary>
        public decimal Value { get; }

        /// <summary>
        /// The <see cref="Value"/> decimal value converted to a string that represent a currency amount.
        /// </summary>
        public string CurrencyAmount { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="value">The price decimal value.</param>
        /// <param name="provider">The format provider for the price currency amount.</param>
        public PriceInfo(decimal value, IFormatProvider provider)
        {
            // Set properties
            Value = value;

            // If format provider is passed...
            if (provider != null)
            {
                SetFormatProvider(provider);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the price <see cref="CurrencyAmount"/> format accordingly to the specified provider.
        /// </summary>
        /// <param name="provider">The format provider for the price currency amount.</param>
        public void SetFormatProvider(IFormatProvider provider)
        {
            CurrencyAmount = Value.ToString("C", provider);
        }

        #endregion
    }
}
