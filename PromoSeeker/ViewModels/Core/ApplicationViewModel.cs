using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PromoSeeker
{
    /// <summary>
    /// The application main view model.
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The tracking products collection.
        /// </summary>
        public ObservableCollection<ProductViewModel> Products { get; set; }
            = new ObservableCollection<ProductViewModel>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows a children window to the user.
        /// </summary>
        /// <param name="onClose">The action to be executed when the window was closed.</param>
        /// <typeparam name="T">The window type to be created.</typeparam>
        public void ShowWindow<T>(BaseViewModel viewModel, Action<object> onClose = null)
            where T : Window
        {
            // Attempt to find the window in the currently initialized windows in our application
            var window = Application.Current.Windows.OfType<T>().FirstOrDefault();

            // If none window was found...
            if (window == null)
            {
                // Create new window
                window = Activator.CreateInstance<T>();
                window.Owner = Application.Current.MainWindow;
                window.DataContext = viewModel;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.ResizeMode = ResizeMode.NoResize;

                // If we have close callback...
                if (onClose != null)
                {
                    window.Closed += (s, e) => onClose(s);
                }
            }

            // Show
            window.ShowDialog();
        }

        /// <summary>
        /// Closes all windows of a given type.
        /// </summary>
        /// <typeparam name="T">The type of a window to be closed.</typeparam>
        public void CloseAllWindow<T>()
            where T : Window
        {
            Application.Current.Windows.OfType<T>().ToList().ForEach(_ => _.Close());
        }

        #endregion
    }
}
