using PromoSeeker.Core;
using System;
using System.Collections.Generic;

namespace PromoSeeker
{
    /// <summary>
    /// The design-time data model for a <see cref="NotificationsViewModel"/>.
    /// </summary>
    public class NotificationsDesignModel : NotificationsViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model.
        /// </summary>
        public static NotificationsDesignModel Instance = new NotificationsDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public NotificationsDesignModel()
        {
            // Fill the list
            Items = new List<NotificationItemViewModel>
            {
                new NotificationItemDesignModel(),
                new NotificationItemDesignModel()
                {
                    IsNew = true,
                    Type = NotificationType.NameChange
                },
                new NotificationItemDesignModel()
                {
                    Type = NotificationType.NameChange
                },
                new NotificationItemDesignModel(),
                new NotificationItemDesignModel(),
            };
        }

        #endregion
    }
}
