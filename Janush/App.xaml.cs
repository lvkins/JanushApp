using Janush.Core;
using System;
using System.Threading;
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
            DI.Application.Load();

            // Monitor connection
            StartConnectionMonitor();
        }

        /// <summary>
        /// Handles the application exit, including the state handling. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            // Save current state on exit
            DI.Application.Save();

            // Dispose mutex
            Mutex?.Dispose();

            base.OnExit(e);
        }

        private void StartConnectionMonitor()
        {
            ConnectionMonitor = new ConnectionMonitor(TimeSpan.FromSeconds(30), state =>
            {
                DI.Application.IsOnline = state;
            });
        }
    }
}