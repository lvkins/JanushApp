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
            // Workaround fit to the current window height. Not working perfectly! TODO: Fix me
            LogsTable.GetBindingExpression(HeightProperty).UpdateTarget();
        }
    }
}
