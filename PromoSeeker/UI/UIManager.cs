using System.Threading.Tasks;
using System.Windows;

namespace PromoSeeker
{
    /// <summary>
    /// A class containing methods for handling the UI interactions in the application.
    /// </summary>
    public class UIManager : IUIManager
    {
        #region Properties

        #endregion

        #region Public Properties

        /// <summary>
        /// The main application tray icon.
        /// </summary>
        public TrayIcon Tray { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the UI components.
        /// </summary>
        public void Initialize()
        {
            // Initialize tray icon
            Tray = new TrayIcon();
        }

        /// <summary>
        /// Shows a single message box dialog to the user.
        /// </summary>
        /// <param name="viewModel">The view model representing the <see cref="MessageBoxDialog"/>.</param>
        /// <returns>A task that will finish once the dialog is closed.</returns>
        public async Task ShowMessageDialogBoxAsync(MessageDialogBoxViewModel viewModel)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return new MessageBoxDialog().ShowDialog(viewModel);
            });
        }

        /// <summary>
        /// Displays a single confirmation message box to the user
        /// </summary>
        /// <param name="viewModel">The view model representing the <see cref="ConfirmBoxDialog"/>.</param>
        /// <returns>A task that will finish once the dialog is closed. Task result contains <see cref="true"/> if confirmation response was successful, otherwise <see cref="false"/>.</returns>
        public async Task<bool> ShowConfirmDialogBoxAsync(ConfirmDialogBoxViewModel viewModel)
        {
            // Run on the UI dispatcher thread
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                // Show dialog
                return new ConfirmBoxDialog().ShowDialog(viewModel);
            });

            // Return user response
            return viewModel.Response;
        }

        /// <summary>
        /// Shows a single prompt message box dialog to the user.
        /// </summary>
        /// <param name="viewModel">The view model representing the <see cref="PromptBoxDialog"/>.</param>
        /// <returns>A task that will finish once the dialog is closed, task result contains the user input.</returns>
        public async Task<string> ShowPromptDialogBoxAsync(PromptDialogBoxViewModel viewModel)
        {
            // Await show dialog operation
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return new PromptBoxDialog().ShowDialog(viewModel);
            });

            // Return the user input
            return viewModel.Input;
        }

        #endregion
    }
}