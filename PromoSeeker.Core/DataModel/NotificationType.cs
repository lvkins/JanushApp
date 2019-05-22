namespace PromoSeeker.Core
{
    /// <summary>
    /// Types of the product related notifications.
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// A notification without specific type.
        /// </summary>
        None,

        /// <summary>
        /// Price decrease notification type.
        /// </summary>
        PriceDown,

        /// <summary>
        /// Price increase notification type.
        /// </summary>
        PriceUp,

        /// <summary>
        /// Name change notification type.
        /// </summary>
        NameChange,
    }
}
