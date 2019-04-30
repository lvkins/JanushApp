using System.Threading.Tasks;

namespace PromoSeeker
{
    /// <summary>
    /// A interface containing methods for handling the UI interactions in the application.
    /// </summary>
    public interface IUIManager
    {
        #region Properties

        /// <summary>
        /// The main application tray icon.
        /// </summary>
        TrayIcon Tray { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the UI components.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Shows a single message box dialog to the user.
        /// </summary>
        /// <param name="viewModel">The view model representing the <see cref="MessageBoxDialog"/>.</param>
        /// <returns>A task that will finish once the dialog is closed.</returns>
        Task ShowMessageDialogBoxAsync(MessageDialogBoxViewModel viewModel);

        /// <summary>
        /// Displays a single confirmation message box to the user
        /// </summary>
        /// <param name="viewModel">The view model representing the <see cref="ConfirmBoxDialog"/>.</param>
        /// <returns>A task that will finish once the dialog is closed. Task result contains <see cref="true"/> if confirmation response was successful, otherwise <see cref="false"/>.</returns>
        Task<bool> ShowConfirmDialogBoxAsync(ConfirmDialogBoxViewModel viewModel);

        /// <summary>
        /// Shows a single prompt message box dialog to the user.
        /// </summary>
        /// <param name="viewModel">The view model representing the <see cref="PromptBoxDialog"/>.</param>
        /// <returns>A task that will finish once the dialog is closed, task result contains the user input.</returns>
        Task<string> ShowPromptDialogBoxAsync(PromptDialogBoxViewModel viewModel);

        #endregion
    }
}
