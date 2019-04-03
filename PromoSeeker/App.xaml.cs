using System.Windows;

namespace PromoSeeker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Setup dependency injection
            DI.Setup();

            // Initialize UI components.
            DI.UIManager.Initialize();

            // Log startup
            DI.Logger.Info("Application started");
        }
    }
}