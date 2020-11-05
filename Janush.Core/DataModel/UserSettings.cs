using System;
using System.Collections.Generic;
using System.Security;

namespace Janush.Core
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
        /// If to notify when name change occurs.
        /// </summary>
        public bool NotifyNameChange { get; set; } = true;

        /// <summary>
        /// If to notify when price decrease occurs.
        /// </summary>
        public bool NotifyPriceDecrease { get; set; } = true;

        /// <summary>
        /// If to notify when price increase occurs.
        /// </summary>
        public bool NotifyPriceIncrease { get; set; } = true;

        /// <summary>
        /// If the sound notifications should be used.
        /// </summary>
        public bool SoundNotification { get; set; } = true;

        /// <summary>
        /// Whether to use email notifications.
        /// </summary>
        public bool EmailNotifications { get; set; } = false;

        /// <summary>
        /// The email host setting.
        /// </summary>
        public string EmailHost { get; set; }

        /// <summary>
        /// The email username setting.
        /// </summary>
        public string EmailUsername { get; set; }

        /// <summary>
        /// The email password setting.
        /// </summary>
        public byte[] EmailPassword { get; set; }

        /// <summary>
        /// The secure password entropy.
        /// </summary>
        public byte[] EmailPasswordHash { get; set; }

        /// <summary>
        /// The email port setting.
        /// </summary>
        public string EmailPort { get; set; }

        /// <summary>
        /// Whether if using SSL/TLS for sending emails.
        /// </summary>
        public bool EmailUseTLS { get; set; } = true;

        /// <summary>
        /// Whether if credentials should be used.
        /// </summary>
        public bool EmailUseAuth { get; set; } = true;

        /// <summary>
        /// Whether if the <see cref="UpdateInterval"/> should be randomized by adding a small 
        /// random values to prevent being constant which could lead to various protection 
        /// systems detect some suspicious behavior.
        /// </summary>
        public bool RandomizeInterval { get; set; } = Consts.PRODUCT_UDPATE_INTERVAL_RANDOMIZE;

        /// <summary>
        /// The tracking products.
        /// </summary>
        public List<ProductDataModel> Products { get; set; } = new List<ProductDataModel>();

        /// <summary>
        /// Recent application notifications.
        /// </summary>
        public List<NotificationDataModel> RecentNotifications { get; set; } = new List<NotificationDataModel>();
    }
}
