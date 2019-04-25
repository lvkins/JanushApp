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
        /// <param name="viewModel">The message box view model instance.</param>
        /// <returns>A task that will finish once the dialog is closed.</returns>
        Task ShowMessageBoxAsync(MessageDialogViewModel viewModel);

        /// <summary>
        /// Shows a single prompt message box dialog to the user.
        /// </summary>
        /// <param name="viewModel">The prompt message box view model instance.</param>
        /// <returns>A task that will finish once the dialog is closed.</returns>
        Task<string> ShowPromptMessageBoxAsync(PromptDialogViewModel viewModel);

        #endregion
    }
}
