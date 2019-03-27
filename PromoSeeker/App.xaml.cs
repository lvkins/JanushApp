using System.Windows;

namespace PromoSeeker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// The main application tray icon.
        /// </summary>
        private TrayIcon _trayIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Setup dependency injection
            DI.Setup();

            // Create tray icon
            _trayIcon = new TrayIcon();

            // Log startup
            DI.Logger.Info("Application started");

        }
    }
}