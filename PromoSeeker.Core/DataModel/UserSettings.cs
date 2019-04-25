using System;
using System.Collections.Generic;
using System.Globalization;

namespace PromoSeeker.Core
{
    /// <summary>
    /// The user specific settings.
    /// </summary>
    public sealed class UserSettings
    {
        /// <summary>
        /// If the sound notifications should be used.
        /// </summary>
        public bool SoundNotification { get; set; }

        /// <summary>
        /// The tracking products.
        /// </summary>
        public List<ProductSettings> Products { get; set; } = new List<ProductSettings>();
    }

    /// <summary>
    /// A class representing the product settings data.
    /// </summary>
    public sealed class ProductSettings
    {
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
        /// The current product price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// The product change price history.
        /// </summary>
        public Dictionary<decimal, DateTime> PriceHistory { get; set; }

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
    }
}
