// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

using Janush.Core;
using System;
using System.Windows.Input;

namespace Janush
{
    /// <summary>
    /// A view model for each notification item in the notifications list.
    /// </summary>
    public class NotificationItemViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The notification title for notifications without particular <see cref="Product"/> assigned.
        /// </summary>
        public string Title { get; set; }

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
        /// The date of when notification was received.
        /// </summary>
        public DateTime Date { get; set; } = DateTime.Now;

        /// <summary>
        /// If this is a new notification.
        /// </summary>
        public bool IsNew { get; set; } = true;

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
            // Select the product, if notification is assigned to any
            Product?.Select();

            // Close notifications popup
            DI.Application.NotificationsPopupVisible = false;
        }

        #endregion
    }
}
