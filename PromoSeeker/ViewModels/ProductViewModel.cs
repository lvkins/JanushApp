using PromoSeeker.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

        /// <summary>
        /// The settings object of this product.
        /// </summary>
        private ProductSettings _settings;

        #endregion

        #region Public Properties

        /// <summary>
        /// The product name.
        /// </summary>
        public string Name => !string.IsNullOrWhiteSpace(DisplayName) ? DisplayName : OriginalName;

        /// <summary>
        /// The original name of the product.
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        /// The display, custom name of the product.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// The current price.
        /// </summary>
        public decimal PriceCurrent { get; set; }

        /// <summary>
        /// The product name change history.
        /// </summary>
        public List<KeyValuePair<string, DateTime>> NameHistory { get; set; }

        /// <summary>
        /// The product price change history.
        /// </summary>
        public List<KeyValuePair<decimal, DateTime>> PriceHistory { get; set; }

        /// <summary>
        /// The formatted price value to be displayed as the product price.
        /// </summary>
        public string DisplayPrice => PriceCurrent.ToString("C2", Culture);

        /// <summary>
        /// The culture to use for the currency formatting.
        /// </summary>
        public CultureInfo Culture { get; set; }

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

        /// <summary>
        /// The settings object for this product.
        /// </summary>
        public ProductSettings Settings => _settings;

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

        /// <summary>
        /// The command for deleting the product from the application.
        /// </summary>
        public ICommand DeleteCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor for design time purposes
        /// </summary>
        public ProductViewModel()
        {

        }

        /// <summary>
        /// Settings constructor
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
            DeleteCommand = new RelayCommand(Delete);

            #endregion
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads the product into the view model.
        /// </summary>
        public void Load(ProductSettings settings)
        {
            // Store settings object
            _settings = settings;

            // Populate properties
            OriginalName = settings.Name;
            DisplayName = settings.DisplayName;
            DateAdded = settings.Created;
            LastCheck = settings.LastChecked;
            PriceCurrent = settings.Price.Value;
            Culture = settings.Culture;
            Url = settings.Url;
            Tracked = settings.Tracked;
            NameHistory = settings.NameHistory;
            PriceHistory = settings.PriceHistory;

            // Create product instance
            Product = new Product(settings);

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

            // Set the tracking time interval
            var interval = Consts.PRODUCT_UPDATE_INTERVAL;

            // If tracking interval should be randomized...
            if (Consts.PRODUCT_UDPATE_INTERVAL_RANDOMIZE)
            {
                // Randomize interval
                interval += TimeSpan.FromSeconds(new Random().Next(-6, 6));
            }

            // Start tracking tasks
            await Product.TrackAsync(interval);
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

        /// <summary>
        /// Deletes the product from the application.
        /// </summary>
        public void Delete()
        {
            // Stop any ongoing tracking tasks
            StopTrackingAsync();

            // Delete the product
            DI.Application.DeleteProduct(this);
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
            DI.Logger.Info($"> Update started [{OriginalName}, {Url}]");
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
                DI.Application.NotificationReceived("Failed to update product", this, NotificationType.Warning);
                return;
            }

            #region Notify The User

            // If a product name has changed...
            if (result.HasNewName)
            {
                // Initialize name history dictionary instance
                if (NameHistory == null)
                {
                    NameHistory = new List<KeyValuePair<string, DateTime>>();
                }

                // Append to history
                NameHistory.Add(new KeyValuePair<string, DateTime>(OriginalName, DateTime.Now));

                // Handle notification
                // TODO: localize me
                DI.Application.NotificationReceived($"Product name has changed.\nNew product name:\n\n{Product.Name}",
                    this, popToast: _settings.NotifyPriceChange);
            }

            // If a product price has changed...
            if (result.HasNewPrice)
            {
                // Initialize price history dictionary instance
                if (PriceHistory == null)
                {
                    PriceHistory = new List<KeyValuePair<decimal, DateTime>>();
                }

                // Append to history
                PriceHistory.Add(new KeyValuePair<decimal, DateTime>(PriceCurrent, DateTime.Now));

                // Handle notification
                // TODO: localize me
                DI.Application.NotificationReceived(
                    Product.PriceInfo.Value > PriceCurrent
                    // If new price is higher than current price...
                    ? $"Aww... price has increased!\n\nNew price: {Product.PriceInfo.CurrencyAmount}"
                    // If new price is lower than current price...
                    : $"Ohh... price has decreased!\n\nNew price: {Product.PriceInfo.CurrencyAmount}",
                    this, popToast: _settings.NotifyPriceChange);
            }

            #endregion

            #region Assign New Values

            // Set values
            OriginalName = Product.Name;
            PriceCurrent = Product.PriceInfo.Value;

            // Raise property changed events
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(OriginalName));
            OnPropertyChanged(nameof(PriceCurrent));
            OnPropertyChanged(nameof(DisplayPrice));

            #endregion

            // Save application state after update
            DI.Application.Save();

            // Leave a log message
            DI.Logger.Info($"> Update finished [{OriginalName}]");
        }

        /// <summary>
        /// A callback that occurs when the product tracking task was aborted due to an error.
        /// </summary>
        /// <param name="ex">The exception occurred, if any.</param>
        private void Product_TrackingFailed(Exception ex)
        {
            // TODO:

            // Inform the user

            // Mark the product 'error having'
        }

        #endregion
    }
}
