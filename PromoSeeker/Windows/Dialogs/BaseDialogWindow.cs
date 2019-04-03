using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PromoSeeker
{
    /// <summary>
    /// A base class for the dialog windows in the application.
    /// </summary>
    public partial class BaseDialogWindow : UserControl
    {
        #region Private Members

        /// <summary>
        /// A window instance that holds this dialog window content.
        /// </summary>
        private readonly Window _dialogWindow;

        #endregion

        #region Public Properties

        /// <summary>
        /// The icon for the dialog control.
        /// </summary>
        public ImageSource Icon { get; set; }
            = Imaging.CreateBitmapSourceFromHIcon(SystemIcons.Information.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

        #endregion

        #region Public Commands

        /// <summary>
        /// The command to close the dialog window.
        /// </summary>
        public ICommand CloseCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseDialogWindow()
        {
            // Create window instance
            _dialogWindow = new Window
            {
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            // Create commands
            CloseCommand = new RelayCommand(() => _dialogWindow.Close());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Displays a single dialog window to the user.
        /// </summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="viewModel">The view model that controls this dialog control.</param>
        /// <returns></returns>
        public Task ShowDialog<T>(T viewModel)
            where T : DialogWindowViewModel
        {
            // Create an awaiting task for closing the dialog
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                // Set a valid data context to be used by the control
                DataContext = viewModel;

                // Set the window title
                _dialogWindow.Title = viewModel.Title;

                // Set window content to this dialog control
                _dialogWindow.Content = this;

                // Set window dimensions
                _dialogWindow.MinWidth = viewModel.WindowMinimumWidth;
                _dialogWindow.MinHeight = viewModel.WindowMinimumHeight;

                // Show in the center of the parent window
                _dialogWindow.Owner = Application.Current.MainWindow;

                // Bring window to the front
                _dialogWindow.Topmost = true;
                _dialogWindow.Topmost = false;

                // Show dialog and block-wait until it's closed
                _dialogWindow.ShowDialog();
            }
            finally
            {
                // Inform caller we've finished
                tcs.TrySetResult(true);
            }

            return tcs.Task;
        }

        #endregion
    }
}
