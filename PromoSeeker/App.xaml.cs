using PromoSeeker.Core;
using System.Windows;

namespace PromoSeeker
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

            // Bind logger into the core solution
            CoreDI.Logger = DI.Logger;

            // Initialize UI components.
            DI.UIManager.Initialize();

            // Log startup
            DI.Logger.Info("Application started");

            // Load application
            DI.Application.Load();
        }

        /// <summary>
        /// Handles the application exit, including the state handling. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // Save current state on exit
            DI.Application.Save();
        }
    }
}