using System.Windows;
using System.Windows.Controls;

namespace Janush
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmailPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Update view model
            if (DataContext is SettingsViewModel viewModel)
            {
                viewModel.EmailPassword = EmailPassword.SecurePassword;
            }
        }
    }
}
