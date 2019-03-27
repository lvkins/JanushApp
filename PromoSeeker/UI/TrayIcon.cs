using System;
using System.Drawing;
using System.Windows;
using FormsApp = System.Windows.Forms;

namespace PromoSeeker
{
    /// <summary>
    /// Represents the <see cref="FormsApp.NotifyIcon"/> for our application.
    /// </summary>
    internal class TrayIcon
    {
        #region Private Members

        // An instance of the notify icon.
        private readonly FormsApp.NotifyIcon _trayIcon = new FormsApp.NotifyIcon();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TrayIcon()
        {
            // Retrieve resource icon. TODO: Make use of the Application.Current.MainWindow.Icon?
            //using (var stream = Application.GetResourceStream(new Uri("pack://application:,,,/PromoSeeker;component/Assets/Application.ico")).Stream)
            //{
            //    _trayIcon.Icon = new Icon(stream);
            //}

            // Use application icon
            _trayIcon.Icon = Icon.ExtractAssociatedIcon(Application.ResourceAssembly.ManifestModule.Name);

            // Set tray title
            _trayIcon.Text = Consts.APP_TITLE;

            // Subscribe to the events
            _trayIcon.MouseClick += OnMouseClick;
            Application.Current.Exit += OnAppExit;

            // Make it visible
            _trayIcon.Visible = true;
        }

        #endregion

        /// <summary>
        /// Gets called on application exit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAppExit(object sender, ExitEventArgs e)
        {
            // Dispose tray icon
            _trayIcon.Dispose();
        }

        /// <summary>
        /// Called whenever a tray icon is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseClick(object sender, FormsApp.MouseEventArgs e)
        {
            // Get main window
            var mainWnd = Application.Current.MainWindow;

            // Show window
            mainWnd.Show();
            mainWnd.WindowState = WindowState.Normal;

            // Bring to the top
            mainWnd.Topmost = true;
            mainWnd.Topmost = false;
        }
    }
}
