﻿// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

using System.Threading.Tasks;
using System.Windows;

namespace Janush
{
    /// <summary>
    /// A class containing methods for handling the UI interactions in the application.
    /// </summary>
    public class UIManager : IUIManager
    {
        #region Private Members

        /// <summary>
        /// The main application tray icon.
        /// </summary>
        private TrayIcon _tray;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value that indicates whether the window is active.
        /// </summary>
        public bool MainWindowActive => Application.Current.Dispatcher.Invoke(() => Application.Current.MainWindow.IsActive);

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the UI components.
        /// </summary>
        public void Initialize()
        {
            // Initialize tray icon
            _tray = new TrayIcon();
        }

        /// <summary>
        /// Shows a single message box dialog to the user.
        /// </summary>
        /// <param name="viewModel">The view model representing the <see cref="MessageBoxDialog"/>.</param>
        /// <returns>A task that will finish once the dialog is closed.</returns>
        public async Task ShowMessageDialogBoxAsync(MessageDialogBoxViewModel viewModel)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return new MessageBoxDialog().ShowDialog(viewModel);
            });
        }

        /// <summary>
        /// Displays a single confirmation message box to the user
        /// </summary>
        /// <param name="viewModel">The view model representing the <see cref="ConfirmBoxDialog"/>.</param>
        /// <returns>A task that will finish once the dialog is closed. Task result contains <see cref="true"/> if confirmation response was successful, otherwise <see cref="false"/>.</returns>
        public async Task<bool> ShowConfirmDialogBoxAsync(ConfirmDialogBoxViewModel viewModel)
        {
            // Run on the UI dispatcher thread
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                // Show dialog
                return new ConfirmBoxDialog().ShowDialog(viewModel);
            });

            // Return user response
            return viewModel.Response;
        }

        /// <summary>
        /// Shows a single prompt message box dialog to the user.
        /// </summary>
        /// <param name="viewModel">The view model representing the <see cref="PromptBoxDialog"/>.</param>
        /// <returns>A task that will finish once the dialog is closed, task result contains the user input.</returns>
        public async Task<string> ShowPromptDialogBoxAsync(PromptDialogBoxViewModel viewModel)
        {
            // Await show dialog operation
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return new PromptBoxDialog().ShowDialog(viewModel);
            });

            // Return the user input
            return viewModel.Input;
        }

        /// <summary>
        /// Toggles the marked tray icon.
        /// </summary>
        /// <param name="flag"><see langword="true"/> if marked icon should be used, otherwise <see langword="false"/>.</param>
        public void TrayIndicate(bool flag) => Application.Current.Dispatcher.Invoke(() => _tray.Indicate(flag));

        /// <summary>
        /// Shows a notification near the tray icon.
        /// </summary>
        /// <param name="tipText"></param>
        /// <param name="tipTitle"></param>
        /// <param name="timeout"></param>
        /// <param name="type"></param>
        public void TrayNotification(string tipText, string tipTitle = "",
            ToastNotificationType type = ToastNotificationType.None, int timeout = int.MaxValue)
            => Application.Current.Dispatcher.Invoke(() => _tray.Notification(tipText, tipTitle, type, timeout));

        #endregion
    }
}