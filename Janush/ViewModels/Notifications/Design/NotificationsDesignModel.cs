using Janush.Core;
using System.Collections.ObjectModel;

namespace Janush
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
            Items = new ObservableCollection<NotificationItemViewModel>
            {
                // Notification without particular product
                new NotificationItemViewModel()
                {
                    Title = "Custom notification item",
                    Message = "Sample notification without particular product assigned!",
                    Type = NotificationSubjectType.Warning,
                },
                new NotificationItemDesignModel(),
                new NotificationItemDesignModel()
                {
                    IsNew = true,
                    Type = NotificationSubjectType.NameChange
                },
                new NotificationItemDesignModel()
                {
                    Type = NotificationSubjectType.NameChange
                },
                new NotificationItemDesignModel(),
                new NotificationItemDesignModel(),
            };
        }

        #endregion
    }
}
