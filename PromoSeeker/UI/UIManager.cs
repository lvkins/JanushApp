using System.Threading.Tasks;
using System.Windows;

namespace PromoSeeker
{
    public class UIManager : IUIManager
    {
        #region Private Members

        /// <summary>
        /// The main application tray icon.
        /// </summary>
        private TrayIcon _trayIcon;

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
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task ShowMessageBoxAsync(MessageDialogViewModel viewModel)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                return new MessageBoxDialog().ShowDialog(viewModel);
            });
        }

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