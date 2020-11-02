using Janush.Core;
using System.Windows;

namespace Janush
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Loads the application ready to use.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
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
        }

        /// <summary>
        /// Handles the application exit, including the state handling. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            // Save current state on exit
            DI.Application.Save();

            base.OnExit(e);
        }
    }
}