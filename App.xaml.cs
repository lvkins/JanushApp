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

            var p = new Seeker();
            p.Work();

        }
    }
}
