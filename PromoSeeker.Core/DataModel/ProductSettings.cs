using System;
using System.Collections.Generic;
using System.Globalization;

namespace PromoSeeker.Core
{
    /// <summary>
    /// A class representing the product settings data.
    /// </summary>
    public sealed class ProductSettings
    {
        #region Private Members

        /// <summary>
        /// The price details.
        /// </summary>
        private PriceInfo _price;

        #endregion

        #region Public Properties

        /// <summary>
        /// A product URL.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// The product detected name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The product custom name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The product website culture, used for currency conversions.
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// If the product properties should be detected automatically or manually.
        /// </summary>
        public bool AutoDetect { get; set; } = true;

        /// <summary>
        /// If the product tracking is enabled.
        /// </summary>
        public bool Tracked { get; set; } = true;

        /// <summary>
        /// If a notification about the name change should be sent.
        /// </summary>
        public bool NotifyNameChange { get; set; } = true;

        /// <summary>
        /// If a notification about the price change should be sent.
        /// </summary>
        public bool NotifyPriceChange { get; set; } = true;

        /// <summary>
        /// The creation date.
        /// </summary>
        public DateTime Created { get; set; } = DateTime.Now;

        /// <summary>
        /// The last check date.
        /// </summary>
        public DateTime LastChecked { get; set; }

        /// <summary>
        /// The price details.
        /// </summary>
        public PriceInfo Price
        {
            get => _price;
            set
            {
                // Fix Json deserializer having no clue of the product culture...
                if (string.IsNullOrEmpty(value.CurrencyAmount))
                {
                    // Set price format provider to the product culture
                    value.SetFormatProvider(Culture);
                }

                // Update value
                _price = value;
            }
        }

        /// <summary>
        /// The product name change history.
        /// </summary>
        public List<KeyValuePair<string, DateTime>> NameHistory { get; set; }

        /// <summary>
        /// The product price change history.
        /// </summary>
        public List<KeyValuePair<decimal, DateTime>> PriceHistory { get; set; }

        #endregion
    }
}
