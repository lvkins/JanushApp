using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace PromoSeeker
{
    public class WindowViewModel
    {
        /// <summary>
        /// The window this view model handles.
        /// </summary>
        private readonly Window mWindow;

        /// <summary>
        /// 
        /// </summary>
        public double WindowWidthMin { get; set; } = 100;
        public double WindowHeightMin { get; set; } = 100;

        /// <summary>
        /// The height of the title bar.
        /// </summary>
        public GridLength CaptionHeight { get; } = new GridLength(27);

        #region Commands

        public ICommand ConfigureCommand { get; set; }

        #endregion

        public WindowViewModel(Window window)
        {
            mWindow = window;

            mWindow.MinWidth = WindowWidthMin;
            mWindow.MinHeight = WindowHeightMin;

            mWindow.StateChanged += MWindow_StateChanged;

            // Create commands

            ConfigureCommand = new RelayCommand(() => mWindow.Close());
        }

        private void MWindow_StateChanged(object sender, System.EventArgs e)
        {
            Debug.WriteLine("state changed");
        }
    }
}
