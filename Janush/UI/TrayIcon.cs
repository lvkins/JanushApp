// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

using Janush.Core;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows;
using FormsApp = System.Windows.Forms;

namespace Janush
{
    /// <summary>
    /// Represents the <see cref="FormsApp.NotifyIcon"/> for our application.
    /// </summary>
    public sealed class TrayIcon
    {
        #region Private Members

        /// <summary>
        /// An instance of the notify icon.
        /// </summary>
        private readonly FormsApp.NotifyIcon _trayIcon = new FormsApp.NotifyIcon();

        /// <summary>
        /// The original tray icon associated with the application.
        /// </summary>
        private readonly Icon _originIcon = Icon.ExtractAssociatedIcon(Application.ResourceAssembly.ManifestModule.Name);

        /// <summary>
        /// The marked tray icon.
        /// </summary>
        private Icon _markedIcon;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TrayIcon()
        {
            // Use application icon
            _trayIcon.Icon = _originIcon;

            // Set tray title
            _trayIcon.Text = Consts.APP_TITLE;

            // Subscribe to the events
            _trayIcon.MouseClick += OnMouseClick;
            _trayIcon.BalloonTipClicked += TrayIcon_BalloonTipClicked;
            Application.Current.Exit += OnAppExit;

            // Make it visible
            _trayIcon.Visible = true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows a notification near the tray icon.
        /// </summary>
        /// <param name="tipText"></param>
        /// <param name="tipTitle"></param>
        /// <param name="timeout"></param>
        /// <param name="type"></param>
        public void Notification(string tipText, string tipTitle = "",
            ToastNotificationType type = ToastNotificationType.None, int timeout = int.MaxValue)
            => _trayIcon.ShowBalloonTip(timeout, tipTitle, tipText, (FormsApp.ToolTipIcon)type);

        /// <summary>
        /// Toggles the marked tray icon.
        /// </summary>
        /// <param name="flag"><see langword="true"/> if marked icon should be used, otherwise <see langword="false"/>.</param>
        public void Indicate(bool flag)
        {
            // If marked icon is not initialized...
            if (_markedIcon == null)
            {
                // Draw marked icon

                // Create bitmap from original icon
                var bitmap = _originIcon.ToBitmap();

                // Create graphics
                var g = Graphics.FromImage(bitmap);

                // Set high quality
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.CompositingQuality = CompositingQuality.AssumeLinear;

                // Create path
                var path = new GraphicsPath();

                // Draw mark path
                path.AddString("!", FontFamily.GenericMonospace,
                    (int)System.Drawing.FontStyle.Bold,
                    g.DpiY * 20 / 72, // EM font size
                    new System.Drawing.Point(bitmap.Width - 16, bitmap.Height - 26),
                    new StringFormat());

                // Draw outline
                g.DrawPath(new Pen(Color.Black, 5), path);

                // Fill the path
                g.FillPath(Brushes.DarkOrange, path);

                // Create icon from the handle
                _markedIcon = Icon.FromHandle(bitmap.GetHicon());
            }

            // Set current tray icon
            _trayIcon.Icon = flag ? _markedIcon : _originIcon;
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Called whenever a tray icon is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseClick(object sender, FormsApp.MouseEventArgs e) => ShowApplication();

        /// <summary>
        /// Called whenever balloon tip is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrayIcon_BalloonTipClicked(object sender, System.EventArgs e) => ShowApplication();

        /// <summary>
        /// Gets called on application exit.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAppExit(object sender, ExitEventArgs e) =>
            // Dispose tray icon
            _trayIcon.Dispose();

        /// <summary>
        /// Shows the application window.
        /// </summary>
        private void ShowApplication()
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

        #endregion
    }
}
