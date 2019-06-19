using System;

namespace Janush.Core
{
    /// <summary>
    /// The data model representing a single notification in the application.
    /// </summary>
    public sealed class NotificationDataModel
    {
        /// <summary>
        /// The notification title for notifications without particular <see cref="Product"/> assigned.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The type of the notification.
        /// </summary>
        public NotificationSubjectType Type { get; set; }

        /// <summary>
        /// The date of when notification was received.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// The notification message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The product this notification applies to.
        /// </summary>
        public ProductDataModel Product { get; set; }
    }
}
