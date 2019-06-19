using Janush.Core;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Janush
{
    /// <summary>
    /// The view model for the main application window.
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The window this view model handles.
        /// </summary>
        private readonly Window _window;

        /// <summary>
        /// A collection view for the filtered products collection.
        /// </summary>
        private readonly ICollectionView _productsView;

        /// <summary>
        /// 
        /// </summary>
        private string _filterQuery;

        #endregion

        #region Public Properties

        /// <summary>
        /// The window width.
        /// </summary>
        public double WindowWidth { get; set; } = 500;

        /// <summary>
        /// The window height.
        /// </summary>
        public double WindowHeight { get; set; } = 450;

        /// <summary>
        /// The height of the title bar.
        /// </summary>
        public GridLength CaptionHeight { get; } = new GridLength(27);

        /// <summary>
        /// The current products filter query input.
        /// </summary>
        public string FilterQuery
        {
            get => _filterQuery;
            set
            {
                // Update value
                _filterQuery = value;

                // Refresh product collection view
                _productsView.Refresh();
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// The command to open the <see cref="AddProductWindow"/>.
        /// </summary>
        public ICommand OpenAddProductWindowCommand { get; }

        /// <summary>
        /// The command to open the <see cref="SettingsWindow"/>.
        /// </summary>
        public ICommand OpenSettingsWindowCommand { get; }

        /// <summary>
        /// The command to shutdown the application.
        /// </summary>
        public ICommand ShutdownCommand { get; }

        /// <summary>
        /// The command for when the area outside the popup is clicked.
        /// </summary>
        public ICommand PopupClickawayCommand { get; }

        /// <summary>
        /// The command for disabling tracking of all products.
        /// </summary>
        public ICommand StopTrackingAllCommand { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Window constructor.
        /// </summary>
        /// <param name="window">The window this view model belongs to.</param>
        public MainWindowViewModel(Window window)
        {
            // Don't bother running in design time
            if (window == null || DesignerProperties.GetIsInDesignMode(window))
            {
                return;
            }

            #region Style Window

            // Set the window we are handling
            _window = window;

            // Set window properties
            _window.ResizeMode = ResizeMode.NoResize;
            _window.WindowStartupLocation = WindowStartupLocation.Manual;

            // Hook into deactivated event to hide it on lost focus
            _window.Deactivated += (s, e) =>
            {
                _window.Hide();
            };

            // Set position
            var workingArea = SystemParameters.WorkArea;
            _window.Left = workingArea.Right - _window.Width - 16;
            _window.Top = workingArea.Bottom - _window.Height - 16;

            #endregion

            #region Create Commands

            OpenAddProductWindowCommand = new RelayCommand(DI.AddPromotionViewModel.Open);
            OpenSettingsWindowCommand = new RelayCommand(DI.SettingsViewModel.Open);
            ShutdownCommand = new RelayCommand(Application.Current.Shutdown);
            PopupClickawayCommand = new RelayCommand(PopupClickaway);
            StopTrackingAllCommand = new RelayCommand(async () => await StopTrackingAllAsync());

            #endregion

            // Create collection filter for products
            _productsView = CollectionViewSource.GetDefaultView(DI.Application.Products);
            _productsView.Filter = ProductsFilter;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Occurs when the area outside the popup is being clicked.
        /// </summary>
        private void PopupClickaway()
        {
            DI.Application.NotificationsPopupVisible = false;
        }

        /// <summary>
        /// Stops all products from tracking.
        /// </summary>
        private async Task StopTrackingAllAsync()
        {
            // Iterate over current products
            foreach (var product in DI.Application.Products)
            {
                // Stop tracking
                await product.StopTrackingAsync();
            }

            // Save state
            DI.Application.Save();
        }

        /// <summary>
        /// Filters the displayed products in the products collection.
        /// </summary>
        /// <param name="obj">The currently filtered object.</param>
        /// <returns><see langword="true"/> if product passes the current <see cref="FilterQuery"/> input, otherwise <see langword="false"/>.</returns>
        private bool ProductsFilter(object obj)
        {
            // If query is empty...
            if (string.IsNullOrEmpty(FilterQuery))
            {
                // Object is valid
                return true;
            }

            // If input is not a product...
            if (!(obj is ProductViewModel product))
            {
                // Object is invalid
                return false;
            }

            // Return whether name, price or host contains the query
            return product.Name.ContainsEx(FilterQuery) ||
                    product.DisplayPrice.Contains(FilterQuery) ||
                    product.Url.Host.ContainsEx(FilterQuery);
        }

        #endregion

    }
}
