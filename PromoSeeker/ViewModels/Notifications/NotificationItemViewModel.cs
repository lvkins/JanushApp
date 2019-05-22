using PromoSeeker.Core;
using System;
using System.Windows.Input;

namespace PromoSeeker
{
    /// <summary>
    /// A view model for each notification item in the notifications list.
    /// </summary>
    public class NotificationItemViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The type of this notification.
        /// </summary>
        public NotificationType Type { get; set; }

        /// <summary>
        /// The product this notification applies to.
        /// </summary>
        public ProductViewModel Product { get; set; }

        /// <summary>
        /// The notification message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The notification date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// If this is a new notification.
        /// </summary>
        public bool IsNew { get; set; }

        #endregion
    }
}
