using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Janush
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Link with the view model
            DataContext = new MainWindowViewModel(this);
        }

        /// <summary>
        /// Handles settings button click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // If we have a toggle button...
            if (sender is ToggleButton btn)
            {
                // Context menu is open if toggle button is checked
                SettingsContextMenu.IsOpen = (bool)btn.IsChecked;
                // Set valid data context
                SettingsContextMenu.DataContext = btn.DataContext;
            }
        }

        /// <summary>
        /// Handles settings context menu closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            // Uncheck settings toggle button
            SettingsButton.IsChecked = false;
        }
    }
}
