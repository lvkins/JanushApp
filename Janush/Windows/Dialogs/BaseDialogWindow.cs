using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Janush
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

        /// <summary>
        /// The resize mode behavior of the window.
        /// </summary>
        public ResizeMode ResizeMode { get; set; } = ResizeMode.NoResize;

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
            // Don't do this in design mode
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            // Create window instance
            _dialogWindow = new Window
            {
                ResizeMode = ResizeMode,
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
        /// <returns>A task that will finish once the dialog is closed.</returns>
        public Task ShowDialog<T>(T viewModel)
            where T : BaseDialogWindowViewModel
        {
            // Create an awaiting task for closing the dialog
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                // Set a valid data context to be used by the control
                DataContext = viewModel;

                // Set the window title
                _dialogWindow.Title = viewModel.Title;

                // Set window dimensions
                _dialogWindow.MinWidth = viewModel.WindowMinimumWidth;
                _dialogWindow.MinHeight = viewModel.WindowMinimumHeight;

                // Show in the middle of the parent window (which is currently active, otherwise - main window)
                _dialogWindow.Owner = Application.Current.Windows.OfType<Window>().SingleOrDefault(_ => _.IsActive)
                    ?? (Application.Current.MainWindow.IsLoaded
                        ? Application.Current.MainWindow
                        : null);

                // Set proper startup location
                _dialogWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                //_dialogWindow.WindowStartupLocation = _dialogWindow.Owner != null
                //    ? WindowStartupLocation.CenterOwner
                //    : WindowStartupLocation.CenterScreen;

                // Set window content to this dialog control
                _dialogWindow.Content = this;

                // Bring window to the front
                _dialogWindow.Topmost = true;
                _dialogWindow.Topmost = false;

                // Set the icon basing on the dialog type
                var iconHandle = IntPtr.Zero;

                // Question dialog
                if (viewModel.Type == DialogBoxType.Question)
                    iconHandle = SystemIcons.Question.Handle;
                // Informative dialog
                else if (viewModel.Type == DialogBoxType.Information)
                    iconHandle = SystemIcons.Information.Handle;
                // Warning dialog
                else if (viewModel.Type == DialogBoxType.Warning)
                    iconHandle = SystemIcons.Exclamation.Handle;
                // Error dialog
                else if (viewModel.Type == DialogBoxType.Error)
                    iconHandle = SystemIcons.Error.Handle;

                // If we got an icon handle...
                if (iconHandle != IntPtr.Zero)
                {
                    // Create bitmap icon from the handle
                    Icon = Imaging.CreateBitmapSourceFromHIcon(iconHandle,
                        Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                }

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
