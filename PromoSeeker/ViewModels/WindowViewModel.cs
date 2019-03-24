using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public ObservableCollection<ProductViewModel> Products { get; set; } = new ObservableCollection<ProductViewModel>
        {
            new ProductViewModel
            {
                Name = "Test Product",
                PriceCurrent = 1337
            },
            new ProductViewModel
            {
                Name = "Test Product",
                PriceCurrent = 1337
            },
            new ProductViewModel
            {
                Name = "Test Product",
                PriceCurrent = 1337
            },
        };

        /// <summary>
        /// The height of the title bar.
        /// </summary>
        public GridLength CaptionHeight { get; } = new GridLength(27);

        #region Commands

        /// <summary>
        /// 
        /// </summary>
        public ICommand ConfigureCommand { get; }

        /// <summary>
        /// 
        /// </summary>
        public ICommand OpenSettingsCommand { get; }

        #endregion

        public WindowViewModel(Window window)
        {
            // Don't bother running in design time
            if (window == null || DesignerProperties.GetIsInDesignMode(window))
            {
                return;
            }

            mWindow = window;

            mWindow.MinWidth = WindowWidthMin;
            mWindow.MinHeight = WindowHeightMin;

            mWindow.StateChanged += MWindow_StateChanged;

            // Create commands

            ConfigureCommand = new RelayCommand(() => mWindow.Close());
            OpenSettingsCommand = new RelayCommand(() =>
            {
            });
        }

        private void MWindow_StateChanged(object sender, System.EventArgs e)
        {
            Debug.WriteLine("state changed");
        }
    }
}
