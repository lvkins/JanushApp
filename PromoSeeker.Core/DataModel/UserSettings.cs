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
        /// If the sound notifications should be used.
        /// </summary>
        public bool SoundNotification { get; set; }

        /// <summary>
        /// Interval in which the products are updated.
        /// </summary>
        public TimeSpan UpdateInterval { get; set; }

        /// <summary>
        /// The tracking products.
        /// </summary>
        public List<ProductSettings> Products { get; set; } = new List<ProductSettings>();
    }
}
