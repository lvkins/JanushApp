using PromoSeeker.Core;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PromoSeeker
{
    /// <summary>
    /// A view model for the notifications list.
    /// </summary>
    public class NotificationsViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// Whether there are new notifications available.
        /// </summary>
        private bool _new;

        #endregion

        #region Public Properties

        /// <summary>
        /// A list of notifications.
        /// </summary>
        public ObservableCollection<NotificationItemViewModel> Items { get; set; }

        /// <summary>
        /// The date of when the user has readed the notifications.
        /// </summary>
        public DateTime LastRead { get; set; } = DateTime.Now;

        /// <summary>
        /// Whether there are new notifications available.
        /// </summary>
        public bool New
        {
            get => _new;
            set
            {
                // Update value
                _new = value;

                // Raise property changed event
                OnPropertyChanged(nameof(New));
            }
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command to open the notifications popup.
        /// </summary>
        public ICommand OpenCommand { get; set; }

        /// <summary>
        /// The command to close the notifications popup.
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command to toggle the notifications popup visibility.
        /// </summary>
        public ICommand ToggleCommand { get; set; }

        /// <summary>
        /// The command to load the notifications for the user.
        /// </summary>
        public ICommand LoadCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public NotificationsViewModel()
        {
            // Create commands
            OpenCommand = new RelayCommand(Open);
            CloseCommand = new RelayCommand(Close);
            ToggleCommand = new RelayCommand(Toggle);
            LoadCommand = new RelayCommand(Load);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens the notifications popup.
        /// </summary>
        public void Open()
        {
            // If not loaded yet...
            if (Items == null)
            {
                // Load stored notifications
                Load();
            }

            // Set last read date
            LastRead = DateTime.Now;

            // Opening the popup means acknowledging with the new notifications
            New = false;

            // Remove tray indicator, if any
            DI.UIManager.TrayIndicate(false);

            // Show notifications popup
            DI.Application.NotificationsPopupVisible = true;
        }

        /// <summary>
        /// Closes the notifications popup.
        /// </summary>
        public void Close() => DI.Application.NotificationsPopupVisible = false;

        /// <summary>
        /// Toggles the notifications popup visibility.
        /// </summary>
        public void Toggle()
        {
            if (DI.Application.NotificationsPopupVisible)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        /// <summary>
        /// Loads stored notifications to display.
        /// </summary>
        public void Load()
        {
            // If container is not initialized...
            if (Items == null)
            {
                // Initialize notifications container
                Items = new ObservableCollection<NotificationItemViewModel>();
            }

            // Get stored notifications
            CoreDI.SettingsReader.Settings.RecentNotifications
                // Ensure the order - newest first
                .OrderByDescending(_ => _.Date)
                // Take limited amount
                .Take(Consts.NOTIFICATION_MAX_COUNT)
                .ToList()
                .ForEach(notification =>
            {
                // Resolve linked product view model
                var productViewModel = DI.Application.Products.FirstOrDefault(_ => _.Settings == notification.Product);

                // Add to collection
                Items.Add(new NotificationItemViewModel
                {
                    Title = notification.Title,
                    Product = productViewModel,
                    Type = notification.Type,
                    Message = notification.Message,
                    Date = notification.Date,
                    IsNew = false,
                });
            });

            // Raise property changed
            OnPropertyChanged(nameof(Items));
        }

        /// <summary>
        /// Adds a new notification and also shows a popup notification if needed.
        /// </summary>
        /// <param name="notificationItem">The notification item.</param>
        /// <param name="popToast">Whether to show a toast notification as well.</param>
        public void Add(NotificationItemViewModel notificationItem, bool popToast = true)
        {
            // If not loaded yet...
            if (Items == null)
            {
                // Load stored notifications
                Load();
            }

            // Prevent duplicates
            // If last notification is equal, show tray notification only

            // Get previous notification stored
            var lastNotification = CoreDI.SettingsReader.Settings.RecentNotifications.FirstOrDefault();

            // If notifications are not equal...
            if (lastNotification == null ||
                !lastNotification.Message.Equals(notificationItem.Message, StringComparison.OrdinalIgnoreCase) ||
                lastNotification.Type != notificationItem.Type)
            {
                // Put into the collection using UI thread
                Application.Current.Dispatcher.Invoke(() => Items.Insert(0, notificationItem));

                // Raise property changed
                OnPropertyChanged(nameof(Items));

                // Add to local store
                CoreDI.SettingsReader.Settings.RecentNotifications.Insert(0, new NotificationDataModel
                {
                    Title = notificationItem.Title,
                    Type = notificationItem.Type,
                    Date = notificationItem.Date,
                    Message = notificationItem.Message,
                    Product = notificationItem.Product?.Settings,
                });

                // Update local data store
                CoreDI.SettingsReader.Save();

                // If notifications popup is not open...
                if (!DI.Application.NotificationsPopupVisible)
                {
                    // Flag unread notifications
                    New = true;
                }

                // If main window is not active or notifications popup is not open...
                if (!DI.UIManager.MainWindowActive || !DI.Application.NotificationsPopupVisible)
                {
                    // Indicate new notifications
                    DI.UIManager.TrayIndicate(true);
                }
            }

            // If we have a message to notify and should show a toast notification...
            if (!string.IsNullOrEmpty(notificationItem.Message) && popToast)
            {
                // Toast default type
                var notificationType = ToastNotificationType.None;

                // If we are dealing with special notification
                if (notificationItem.Type == NotificationSubjectType.Warning)
                {
                    // Apply toast type
                    notificationType = ToastNotificationType.Warning;
                }

                // Show tray notification
                DI.UIManager.TrayNotification(notificationItem.Message,
                    notificationItem.Product?.Name ?? notificationItem.Title, notificationType);
            }
        }

        #endregion
    }
}
