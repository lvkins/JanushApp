using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PromoSeeker
{
    /// <summary>
    /// The view model for the main application window.
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The window this view model handles.
        /// </summary>
        private readonly Window _window;

        /// <summary>
        /// If the notifications popup dialog is visible.
        /// </summary>
        private bool _notificationsPopupVisible;

        #endregion

        #region Public Properties

        /// <summary>
        /// The window minimum width.
        /// </summary>
        public double WindowWidthMin { get; set; } = 100;

        /// <summary>
        /// The window minimum height.
        /// </summary>
        public double WindowHeightMin { get; set; } = 100;

        /// <summary>
        /// The height of the title bar.
        /// </summary>
        public GridLength CaptionHeight { get; } = new GridLength(27);

        /// <summary>
        /// The notifications container.
        /// </summary>
        public NotificationsViewModel Notifications { get; set; }

        /// <summary>
        /// If the notifications popup dialog is visible.
        /// </summary>
        public bool NotificationsPopupVisible
        {
            get => _notificationsPopupVisible;
            set
            {
                // Update value
                _notificationsPopupVisible = value;

                // Raise property changed event
                OnPropertyChanged(nameof(NotificationsPopupVisible));
                OnPropertyChanged(nameof(AnyPopupVisible));
            }
        }

        /// <summary>
        /// Defines whether any popup within the main window is currently shown and present. 
        /// </summary>
        public bool AnyPopupVisible => NotificationsPopupVisible;

        #endregion

        #region Commands

        /// <summary>
        /// The command to open the <see cref="AddProductWindow"/>.
        /// </summary>
        public ICommand OpenAddProductWindowCommand { get; }

        /// <summary>
        /// The command to open the <see cref="SettingsWindow"/>.
        /// </summary>
        public ICommand OpenSettingsWindowCommand { get; }

        /// <summary>
        /// Toggles a notification popup dialog visibility state.
        /// </summary>
        public ICommand ToggleNotificationsPopup { get; }

        /// <summary>
        /// The command to shutdown the application.
        /// </summary>
        public ICommand ShutdownCommand { get; }

        /// <summary>
        /// The command for when the area outside the popup is clicked.
        /// </summary>
        public ICommand PopupClickawayCommand { get; }

        /// <summary>
        /// The command for disabling tracking of all products.
        /// </summary>
        public ICommand StopTrackingAllCommand { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Window constructor.
        /// </summary>
        /// <param name="window">The window this view model belongs to.</param>
        public MainWindowViewModel(Window window)
        {
            // Don't bother running in design time
            if (window == null || DesignerProperties.GetIsInDesignMode(window))
            {
                return;
            }

            #region Style Window

            // Set the window we are handling
            _window = window;

            // Set window properties
            _window.MinWidth = WindowWidthMin;
            _window.MinHeight = WindowHeightMin;
            _window.ResizeMode = ResizeMode.NoResize;
            _window.WindowStartupLocation = WindowStartupLocation.Manual;
            _window.Deactivated += (s, e) =>
            {
                _window.Hide();
            };

            var workingArea = SystemParameters.WorkArea;
            _window.Left = workingArea.Right - _window.Width - 16;
            _window.Top = workingArea.Bottom - _window.Height - 16;

            #endregion

            #region Create Commands

            OpenAddProductWindowCommand = new RelayCommand(DI.AddPromotionViewModel.Open);
            OpenSettingsWindowCommand = new RelayCommand(DI.SettingsViewModel.Open);
            ShutdownCommand = new RelayCommand(Application.Current.Shutdown);
            PopupClickawayCommand = new RelayCommand(PopupClickaway);
            ToggleNotificationsPopup = new RelayCommand(() =>
            {
                LoadNotifications();
                NotificationsPopupVisible = !NotificationsPopupVisible;
            });
            StopTrackingAllCommand = new RelayCommand(StopTrackingAll);

            #endregion
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads the recent notifications.
        /// </summary>
        private void LoadNotifications()
        {
            if (Notifications == null)
            {
                Notifications = new NotificationsViewModel();
            }

            Notifications.Load();

            OnPropertyChanged(nameof(Notifications));
        }

        /// <summary>
        /// Occurs when the area outside the popup is being clicked.
        /// </summary>
        private void PopupClickaway()
        {
            NotificationsPopupVisible = false;
        }

        /// <summary>
        /// Stops all products from tracking.
        /// </summary>
        private void StopTrackingAll()
        {
            // Iterate over current products
            foreach (var product in DI.Application.Products)
            {
                // Stop tracking
                product.StopTrackingAsync();
            }
        }

        #endregion

    }
}
