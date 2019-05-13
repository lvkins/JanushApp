using System.Windows;

namespace PromoSeeker
{
    /// <summary>
    /// Interaction logic for LogsWindow.xaml
    /// </summary>
    public partial class LogsWindow : Window
    {
        public LogsWindow()
        {
            InitializeComponent();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Adjust DataGrid height to the grid row height - update binding on window resize
            // Workaround fit to the window available height.
            // NOTE: Not working perfectly! TODO: Fix me
            LogsTable.GetBindingExpression(HeightProperty).UpdateTarget();
        }
    }
}
