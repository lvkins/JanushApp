﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PromoSeeker
{
    /// <summary>
    /// The view model for the main application window.
    /// </summary>
    public class WindowViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The window this view model handles.
        /// </summary>
        private readonly Window _window;

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public double WindowWidthMin { get; set; } = 100;
        public double WindowHeightMin { get; set; } = 100;

        /// <summary>
        /// Whether if the settings popup menu is currently visible.
        /// </summary>
        public bool ShowSettingsPopupMenu { get; set; }

        public ObservableCollection<ProductViewModel> Products { get; set; } = new ObservableCollection<ProductViewModel>
        {
            new ProductViewModel
            {
                Name = "Test Product",
                PriceCurrent = 1337
            },
            new ProductViewModel
            {
                Name = "Test Product",
                PriceCurrent = 1337
            },
            new ProductViewModel
            {
                Name = "Test Product",
                PriceCurrent = 1337
            },
        };

        /// <summary>
        /// The height of the title bar.
        /// </summary>
        public GridLength CaptionHeight { get; } = new GridLength(27);

        #endregion

        #region Commands

        /// <summary>
        /// The command ...
        /// </summary>
        public ICommand ConfigureCommand { get; }

        /// <summary>
        /// The command to open the <see cref="SettingsWindow"/>.
        /// </summary>
        public ICommand OpenSettingsWindowCommand { get; }

        /// <summary>
        /// The command to toggle the settings popup visibility.
        /// </summary>
        public ICommand ToggleSettingsPopupCommand { get; }

        /// <summary>
        /// The command to shutdown the application.
        /// </summary>
        public ICommand ShutdownCommand { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Window constructor.
        /// </summary>
        /// <param name="window">The window this view model belongs to.</param>
        public WindowViewModel(Window window)
        {
            // Don't bother running in design time
            if (window == null || DesignerProperties.GetIsInDesignMode(window))
            {
                return;
            }

            // Set the window we are handling
            _window = window;

            // Set window properties
            _window.MinWidth = WindowWidthMin;
            _window.MinHeight = WindowHeightMin;
            _window.ResizeMode = ResizeMode.NoResize;
            _window.WindowStartupLocation = WindowStartupLocation.Manual;
            //_window.Hide();

            var workingArea = SystemParameters.WorkArea;
            _window.Left = workingArea.Right - _window.Width - 16;
            _window.Top = workingArea.Bottom - _window.Height - 16;

            #region Create Commands

            ConfigureCommand = new RelayCommand(() => _window.Close());

            ToggleSettingsPopupCommand = new RelayCommand(() =>
            {
                // Toggle settings popup visibility
                ShowSettingsPopupMenu = !ShowSettingsPopupMenu;

                // Notify property changed
                OnPropertyChanged(nameof(ShowSettingsPopupMenu));
            });

            OpenSettingsWindowCommand = new RelayCommand(() => ShowChildWindow<SettingsWindow>());

            ShutdownCommand = new RelayCommand(() => Application.Current.Shutdown());

            #endregion
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Shows a children window to the user.
        /// </summary>
        /// <param name="onClose">The action to be executed when the window was closed.</param>
        /// <typeparam name="T">The window type to be created.</typeparam>
        private void ShowChildWindow<T>(Action<object> onClose = null)
            where T : Window
        {
            // Attempt to find the window in the currently initialized windows in our application
            var window = Application.Current.Windows.OfType<T>().FirstOrDefault();

            // If none window was found...
            if (window == null)
            {
                // Create new window
                window = Activator.CreateInstance<T>();
                window.Owner = _window;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                window.ResizeMode = ResizeMode.NoResize;

                // If we have close callback...
                if (onClose != null)
                {
                    window.Closed += (s, e) => onClose(s);
                }
            }

            // Show
            window.ShowDialog();
        }

        #endregion
    }
}
