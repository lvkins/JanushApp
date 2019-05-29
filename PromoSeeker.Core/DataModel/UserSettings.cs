using Newtonsoft.Json;
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
        /// Interval in which the products are updated.
        /// </summary>
        public TimeSpan UpdateInterval { get; set; } = Consts.PRODUCT_UPDATE_INTERVAL;

        /// <summary>
        /// If the sound notifications should be used.
        /// </summary>
        public bool SoundNotification { get; set; } = true;

        /// <summary>
        /// Whether if the <see cref="UpdateInterval"/> should be randomized by adding a small 
        /// random values to prevent being constant which could lead to various protection 
        /// systems detect some suspicious behavior.
        /// </summary>
        public bool RandomizeInterval { get; set; } = Consts.PRODUCT_UDPATE_INTERVAL_RANDOMIZE;

        /// <summary>
        /// The tracking products.
        /// </summary>
        public List<ProductSettings> Products { get; set; } = new List<ProductSettings>();
    }
}
