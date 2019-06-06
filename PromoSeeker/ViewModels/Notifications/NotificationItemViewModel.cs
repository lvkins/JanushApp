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
        public NotificationSubjectType Type { get; set; }

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

        #region Public Commands

        /// <summary>
        /// The command to navigate to the notification product website.
        /// </summary>
        public ICommand NavigateCommand { get; set; }

        /// <summary>
        /// The command to select the notification product in the list.
        /// </summary>
        public ICommand SelectCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public NotificationItemViewModel()
        {
            // Create commands
            NavigateCommand = new RelayCommand(Navigate);
            SelectCommand = new RelayCommand(Select);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Navigates to the product's website.
        /// </summary>
        private void Navigate()
        {
            // Navigate
            Product?.Navigate();

            // Close notifications popup
            DI.Application.NotificationsPopupVisible = false;
        }

        /// <summary>
        /// Selects the product in the overall products list.
        /// </summary>
        private void Select()
        {
            // Select
            Product?.Select();

            // Close notifications popup
            DI.Application.NotificationsPopupVisible = false;
        }

        #endregion
    }
}
