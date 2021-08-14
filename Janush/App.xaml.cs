// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

using Janush.Core;
using Janush.Core.Localization;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Janush
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// The application mutex.
        /// </summary>
        private static Mutex Mutex = null;

        /// <summary>
        /// The connection monitor for the app.
        /// </summary>
        private static ConnectionMonitor ConnectionMonitor;

        /// <summary>
        /// The application unique identifier.
        /// </summary>
        private static readonly string AppGuid = "73f73810-b345-4b3e-a713-e7671cf1f50b";

        /// <summary>
        /// Loads the application ready to use.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // Create mutex
            Mutex = new Mutex(false, "Global\\" + AppGuid, out var isNewMutex);

            if (!isNewMutex)
            {
                // Terminate process
                Environment.Exit(0);
                return;
            }

            base.OnStartup(e);

            // Setup dependency injection
            DI.Setup();

            // Register a provider for the class library
            CoreDI.Provider = DI.Provider;

            // Initialize UI components.
            DI.UIManager.Initialize();

            // Log startup
            CoreDI.Logger.Info("Application started");

            // Load application
            DI.Application.LoadAsync();

            // Monitor connection
            StartConnectionMonitor();

            // Check for new version
            CheckVersionAsync();
        }


        /// <summary>
        /// Handles the application exit, including the state handling. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            // Save current state on exit
            DI.Application.Save();

            // Exit browser
            _ = CoreDI.Browser.DisposeAsync();

            // Dispose mutex
            Mutex?.Dispose();

            base.OnExit(e);
        }

        /// <summary>
        /// Starts connection monitor task.
        /// </summary>
        private void StartConnectionMonitor()
        {
            ConnectionMonitor = new ConnectionMonitor(Consts.ConnectionMonitorInterval, state =>
            {
                DI.Application.IsOnline = state;
            });
        }

        /// <summary>
        /// Checks for new application version.
        /// </summary>
        private async void CheckVersionAsync()
        {
            // Let application show
            await Task.Delay(TimeSpan.FromSeconds(4));

            // If new version is available...
            if (await VersionMonitor.IsNewAsync())
            {
                // Prompt user
                await DI.UIManager.ShowConfirmDialogBoxAsync(new ConfirmDialogBoxViewModel
                {
                    Title = Strings.NewVersionDialogTitle,
                    Message = string.Format(Strings.NewVersionDialogContent, Consts.APP_TITLE),
                    OnAccept = () =>
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = Consts.APP_DOWNLOAD_URL,
                            UseShellExecute = true
                        });
                    },
                    OkText = Strings.Download,
                    CancelText = Strings.Close
                });
            }
        }
    }
}