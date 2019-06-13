using PromoSeeker.Core.Localization;
using System;

namespace PromoSeeker.Core
{
    /// <summary>
    /// The types of the product status.
    /// </summary>
    public enum ProductTrackingStatusType
    {
        /// <summary>
        /// The tracking awaits activation.
        /// </summary>
        Idle,

        /// <summary>
        /// Tracking is running.
        /// </summary>
        Tracking,

        /// <summary>
        /// Fresh product data is currently being pulled.
        /// </summary>
        Updating,

        /// <summary>
        /// Product tracking has been disabled.
        /// </summary>
        Disabled,

        /// <summary>
        /// An error occurred during tracking.
        /// </summary>
        Error,
    }

    /// <summary>
    /// The extension methods for <see cref="ProductTrackingStatusType"/>.
    /// </summary>
    public static class ProductTrackingStatusTypeExtensions
    {
        /// <summary>
        /// Translates the enum type to the localized display string.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToDisplayString(this ProductTrackingStatusType type)
        {
            switch (type)
            {
                case ProductTrackingStatusType.Idle: return Strings.TrackingStatusIdle;
                case ProductTrackingStatusType.Tracking: return Strings.TrackingStatusActive;
                case ProductTrackingStatusType.Updating: return Strings.TrackingStatusUpdating;
                case ProductTrackingStatusType.Error: return Strings.TrackingStatusError;
                case ProductTrackingStatusType.Disabled: return Strings.TrackingStatusDisabled;
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
