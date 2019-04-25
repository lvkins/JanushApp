using System.Threading.Tasks;
using System.Windows;

namespace PromoSeeker
{
    /// <summary>
    /// A class containing methods for handling the UI interactions in the application.
    /// </summary>
    public class UIManager : IUIManager
    {
        #region Private Members

        /// <summary>
        /// The main application tray icon.
        /// </summary>
        private TrayIcon _trayIcon;

        #endregion

        #region Public Properties

        /// <summary>
        /// The main application tray icon.
        /// </summary>
        public TrayIcon Tray => _trayIcon;

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the UI components.
        /// </summary>
        public void Initialize()
        {
            // Create tray icon
            _trayIcon = new TrayIcon();
        }

        /// <summary>
        /// Shows a single message box dialog to the user.
        /// </summary>
        /// <param name="viewModel">The message box view model instance.</param>
        /// <returns>A task that will finish once the dialog is closed.</returns>
        public async Task ShowMessageBoxAsync(MessageDialogViewModel viewModel)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return new MessageBoxDialog().ShowDialog(viewModel);
            });
        }

        /// <summary>
        /// Shows a single prompt message box dialog to the user.
        /// </summary>
        /// <param name="viewModel">The prompt message box view model instance.</param>
        /// <returns>A task that will finish once the dialog is closed.</returns>
        public async Task<string> ShowPromptMessageBoxAsync(PromptDialogViewModel viewModel)
        {
            // Await show dialog operation
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return new PromptDialog().ShowDialog(viewModel);
            });

            // Return the user input
            return viewModel.Input;
        }

        #endregion
    }
}