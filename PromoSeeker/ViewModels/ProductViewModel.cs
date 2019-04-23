using PromoSeeker.Core;
using System;
using System.Diagnostics;
using System.Timers;
using System.Windows.Input;

namespace PromoSeeker
{
    /// <summary>
    /// The view model representing the product in the tracker.
    /// </summary>
    public class ProductViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// Date when the product was checked.
        /// </summary>
        private DateTime _lastCheck;

        /// <summary>
        /// A timer for raising the property changed of the <see cref="LastCheck"/> property.
        /// </summary>
        private readonly Timer _lastCheckTimer;

        /// <summary>
        /// If the product options popup menu is currently expanded.
        /// </summary>
        private bool _showOptionsPopupMenu;

        /// <summary>
        /// Whether the product is currently being updated.
        /// </summary>
        private bool _currentlyUpdating;

        /// <summary>
        /// If the tracking is enabled for this product.
        /// </summary>
        private bool _tracked;

        #endregion

        #region Public Properties

        /// <summary>
        /// The original name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The display, custom name of the product.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The current price.
        /// </summary>
        public decimal PriceCurrent { get; set; }

        /// <summary>
        /// Date when the product was created.
        /// </summary>
        public DateTime DateAdded { get; set; }

        /// <summary>
        /// Date when the product was checked.
        /// </summary>
        public DateTime LastCheck
        {
            get => _lastCheck;
            set
            {
                // Update value
                _lastCheck = value;

                // Raise property changed
                OnPropertyChanged(nameof(LastCheck));
            }
        }

        /// <summary>
        /// The display URL.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// The product instance.
        /// </summary>
        public Product Product { get; internal set; }

        /// <summary>
        /// If the product options popup menu is currently expanded.
        /// </summary>
        public bool ShowOptionsPopupMenu
        {
            get => _showOptionsPopupMenu;
            set
            {
                // Update value
                _showOptionsPopupMenu = value;

                // Raise property changed event
                OnPropertyChanged(nameof(ShowOptionsPopupMenu));
            }
        }

        /// <summary>
        /// Whether the product instance is set and loaded.
        /// </summary>
        public bool IsLoaded => Product?.IsLoaded == true;

        /// <summary>
        /// Whether the product is currently being updated.
        /// </summary>
        public bool CurrentlyUpdating
        {
            get => _currentlyUpdating;
            set
            {
                // Update value
                _currentlyUpdating = value;

                // Raise property changed
                OnPropertyChanged(nameof(CurrentlyUpdating));
            }
        }

        /// <summary>
        /// If the tracking is enabled for this product.
        /// </summary>
        public bool Tracked
        {
            get => _tracked;
            set
            {
                // Update value
                _tracked = value;

                // Raise property changed
                OnPropertyChanged(nameof(Tracked));
            }
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command for loading the product.
        /// </summary>
        public ICommand LoadCommand { get; set; }

        /// <summary>
        /// The command for opening the product in the browser.
        /// </summary>
        public ICommand NavigateCommand { get; set; }

        /// <summary>
        /// The command for expanding the product options popup menu.
        /// </summary>
        public ICommand ToggleOptionsPopupCommand { get; set; }

        /// <summary>
        /// The command for starting the product tracking task.
        /// </summary>
        public ICommand StartTrackingCommand { get; set; }

        /// <summary>
        /// The command for stopping the product tracking task.
        /// </summary>
        public ICommand StopTrackingCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProductViewModel(ProductSettings product)
        {
            // If we have no product settings...
            if (product == null)
            {
                throw new ArgumentNullException("Product settings are required");
            }

            // Load product from settings
            Load(product);

            // Create the last check refreshing timer
            _lastCheckTimer = new Timer
            {
                AutoReset = true,
                Interval = 60 * 1000,
                Enabled = true,
            };

            _lastCheckTimer.Elapsed += (s, e) =>
            {
                OnPropertyChanged(nameof(LastCheck));
            };

            #region Create Commands

            LoadCommand = new RelayParamCommand((param) => Load((ProductSettings)param));
            NavigateCommand = new RelayCommand(() => Process.Start(Url.ToString()));
            ToggleOptionsPopupCommand = new RelayCommand(() =>
            {
                // Toggle value
                ShowOptionsPopupMenu = !ShowOptionsPopupMenu;
            });
            StartTrackingCommand = new RelayCommand(StartTrackingAsync);
            StopTrackingCommand = new RelayCommand(StopTrackingAsync);

            #endregion
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads the product into the view model.
        /// </summary>
        public void Load(ProductSettings product)
        {
            // Set properties
            DisplayName = !string.IsNullOrWhiteSpace(product.DisplayName)
                ? product.DisplayName
                : product.Name;
            Name = product.Name;
            DateAdded = product.Created;
            LastCheck = product.LastChecked;
            PriceCurrent = product.Price;
            Url = product.Url;
            Tracked = product.Tracked;

            // Create product instance
            Product = new Product(Url.ToString());
            // Subscribe to the events
            Product.TrackingFailed += Product_TrackingFailed;
            Product.Updating += Product_Updating;
            Product.Updated += Product_Updated;

            // If the product tracking is enabled...
            if (Tracked)
            {
                // Start tracking
                StartTrackingAsync();
            }
        }

        /// <summary>
        /// Starts the tracking task for this product.
        /// </summary>
        public async void StartTrackingAsync()
        {
            // Close options popup
            ShowOptionsPopupMenu = false;

            // Flag as tracked
            Tracked = true;

            // Start tracking tasks
            await Product.TrackAsync(Consts.PRODUCT_UPDATE_INTERVAL);
        }

        /// <summary>
        /// Disable this product from tracking.
        /// </summary>
        public async void StopTrackingAsync()
        {
            // Close options popup
            ShowOptionsPopupMenu = false;

            // Stop any tracking tasks
            await Product.StopTrackingAsync();

            // Once stopped, flag as not tracked
            Tracked = false;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// A callback that occurs whenever the product update has started.
        /// </summary>
        private void Product_Updating()
        {
            // Flag we are updating
            CurrentlyUpdating = true;

            // Leave a log message
            DI.Logger.Info($"> Update [{Name}, {Url}]");
        }

        /// <summary>
        /// A callback that occurs whenever the product is updated.
        /// </summary>
        private void Product_Updated(ProductUpdateResult result)
        {
            // Update last check time
            LastCheck = DateTime.Now;
            // Flag we are no longer updating
            CurrentlyUpdating = false;

            // If the update wasn't successful...
            if (!result.Success)
            {
                // TODO: Handle unsuccessful update
            }

            // Save application state
            DI.Application.Save();

            // Leave a log message
            DI.Logger.Info($"> Update finished [{Name}]");
        }

        /// <summary>
        /// A callback that occurs when the product tracking task was aborted due to an error.
        /// </summary>
        /// <param name="ex">The exception occurred, if any.</param>
        private void Product_TrackingFailed(Exception ex)
        {
            // Inform the user

            // Mark the product 'error having'
        }


        #endregion
    }
}
