using System;
using System.Collections.Generic;

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
    /// A promotion settings.
    /// </summary>
    public sealed class ProductSettings
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The product
        /// </summary>
        public Dictionary<decimal, DateTime> PriceHistory { get; set; }

        /// <summary>
        /// A promotion URL.
        /// </summary>
        public string Url { get; set; }
    }
}
