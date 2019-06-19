namespace Janush.Core
{
    /// <summary>
    /// Types of the product related notifications subject.
    /// </summary>
    public enum NotificationSubjectType
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

        /// <summary>
        /// A warning notification type.
        /// </summary>
        Warning,
    }
}
