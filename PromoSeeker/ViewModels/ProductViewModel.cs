using LiveCharts;
using LiveCharts.Wpf;
using PromoSeeker.Core;
using PromoSeeker.Core.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace PromoSeeker
{
    /// <summary>
    /// The view model representing the product in the application.
    /// </summary>
    public class ProductViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The culture to use for the currency formatting.
        /// </summary>
        private CultureInfo _culture;

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
        /// If the product is selected in the UI.
        /// </summary>
        private bool _isSelected;

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
        public string DisplayPrice => PriceCurrent.ToString(Consts.CURRENCY_FORMAT, Culture);

        /// <summary>
        /// The highest price along with the <see cref="DateTime"/> of when it occurred.
        /// </summary>
        public KeyValuePair<decimal, DateTime> HighestPrice
        {
            get
            {
                // Find highest price history entry
                var historyPrice = PriceHistory?.Aggregate((seed, next) => seed.Key > next.Key ? seed : next);

                // If we don't have pricing history...
                return historyPrice == null
                    // Return current price as the highest
                    ? new KeyValuePair<decimal, DateTime>(PriceCurrent, Settings.Created)
                    // Return highest price in the history
                    : historyPrice.Value;
            }
        }

        /// <summary>
        /// The lowest price along with the <see cref="DateTime"/> of when it occurred.
        /// </summary>
        public KeyValuePair<decimal, DateTime> LowestPrice
        {
            get
            {
                // Find lowest price history entry
                var historyPrice = PriceHistory?.Aggregate((seed, next) => seed.Key < next.Key ? seed : next);

                // If we don't have pricing history...
                return historyPrice == null
                    // Return current price as the lowest
                    ? new KeyValuePair<decimal, DateTime>(PriceCurrent, Settings.Created)
                    // Return lowest price in the history
                    : historyPrice.Value;
            }
        }

        /// <summary>
        /// The culture to use for the currency formatting.
        /// </summary>
        public CultureInfo Culture
        {
            get => _culture;
            set
            {
                // Update value
                _culture = value;

                // Update region for the culture
                CultureRegion = new RegionInfo(_culture.Name);
            }
        }

        /// <summary>
        /// The region info for the current product <see cref="Culture"/>.
        /// </summary>
        public RegionInfo CultureRegion { get; private set; }

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
        /// The settings object for this product.
        /// </summary>
        public ProductSettings Settings { get; private set; }

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
                OnPropertyChanged(nameof(TrackingStatus));
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
                OnPropertyChanged(nameof(TrackingStatus));
            }
        }

        /// <summary>
        /// The tracking status of this product.
        /// </summary>
        public ProductTrackingStatusType TrackingStatus
        {
            get
            {
                // If product is updating...
                if (CurrentlyUpdating)
                {
                    return ProductTrackingStatusType.Updating;
                }

                // If we are not tracking this product...
                if (!Tracked)
                {
                    return ProductTrackingStatusType.Disabled;
                }

                // TODO: return more types

                // If tracking task is active...
                return Product?.IsTrackingRunning == true
                    // Product is tracked
                    ? ProductTrackingStatusType.Tracking
                    // Product is idle
                    : ProductTrackingStatusType.Idle;
            }
        }

        /// <summary>
        /// If the product is selected in the overall product list.
        /// This is used to highlight the product and if set to <see langword="true"/>.
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                // Update value
                _isSelected = value;

                // Raise property changed
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        /// <summary>
        /// Whether the product instance is set and loaded.
        /// </summary>
        public bool IsLoaded => Product?.IsLoaded == true;

        #region Charts

        /// <summary>
        /// The money formatted callback. Formats the input using the current
        /// product <see cref="Culture"/> and returns formatted currency amount.
        /// </summary>
        public Func<double, string> MoneyFormatter { get; set; }

        /// <summary>
        /// The Y axis labels.
        /// </summary>
        public List<string> Labels { get; private set; }

        /// <summary>
        /// The line plot container.
        /// </summary>
        public SeriesCollection SeriesViews { get; set; }

        #endregion

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
        /// The command for selecting the product in the list.
        /// </summary>
        public ICommand SelectCommand { get; set; }

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
        /// The command for showing a product overall details page.
        /// </summary>
        public ICommand ShowDetailsPageCommand { get; set; }

        /// <summary>
        /// The command for closing a product overall details page.
        /// </summary>
        public ICommand HideDetailsPageCommand { get; set; }

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

            #region Charts

            // Create chart data to display; if product doesn't have a price history
            // use current price as default
            var chartData = PriceHistory ?? new List<KeyValuePair<decimal, DateTime>>
            {
                new KeyValuePair<decimal, DateTime>(PriceCurrent, Settings.Created)
            };

            // Create money formatter callback
            MoneyFormatter = (input) => input.ToString(Consts.CURRENCY_FORMAT, Culture);

            // Set x-axis date labels
            Labels = chartData.Select(_ => _.Value.ToShortDateString()).ToList();

            // Create chart
            SeriesViews = new SeriesCollection
            {
                new LineSeries
                {
                    Title = Strings.Price,
                    Values = new ChartValues<decimal>(chartData.Select(_ => _.Key)),
                    DataLabels = true
                }
            };

            #endregion

            #region Last check timer

            // Create the last check refreshing timer
            _lastCheckTimer = new Timer
            {
                AutoReset = true,
                Interval = 60 * 1000,
                Enabled = true,
            };

            _lastCheckTimer.Elapsed += (s, e) => OnPropertyChanged(nameof(LastCheck));

            #endregion

            #region Create Commands

            LoadCommand = new RelayParamCommand((param) => Load((ProductSettings)param));
            NavigateCommand = new RelayCommand(Navigate);
            SelectCommand = new RelayCommand(Select);
            ToggleOptionsPopupCommand = new RelayCommand(() =>
            {
                // Toggle value
                ShowOptionsPopupMenu = !ShowOptionsPopupMenu;
            });
            StartTrackingCommand = new RelayCommand(StartTrackingAsync);
            StopTrackingCommand = new RelayCommand(async () => await StopTrackingAsync());
            ShowDetailsPageCommand = new RelayCommand(ShowDetailsPage);
            HideDetailsPageCommand = new RelayCommand(HideDetailsPage);
            DeleteCommand = new RelayCommand(async () => await DeleteAsync());

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
            Settings = settings;

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
        /// Navigates to the product website.
        /// </summary>
        public void Navigate()
        {
            // Start navigating
            Process.Start(Url.ToString());

            // Close popup menu
            ShowOptionsPopupMenu = false;
        }

        /// <summary>
        /// Selects the product in the overall products list.
        /// </summary>
        public void Select()
        {
            // Close popup menu
            ShowOptionsPopupMenu = false;

            // Set selected product
            DI.Application.SelectedProduct = this;
        }

        /// <summary>
        /// Shows the details page for this product.
        /// </summary>
        public void ShowDetailsPage()
        {
            // Close popup menu
            ShowOptionsPopupMenu = false;

            // Assign details page view model
            DI.Application.CurrentProductDetails = this;
        }

        /// <summary>
        /// Hides the details page.
        /// </summary>
        public void HideDetailsPage() =>
            DI.Application.CurrentProductDetails = null;

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
            await Product.TrackAsync(CoreDI.SettingsReader.Settings.UpdateInterval,
                CoreDI.SettingsReader.Settings.RandomizeInterval);
        }

        /// <summary>
        /// Disable this product from tracking.
        /// </summary>
        public async Task StopTrackingAsync()
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
        public async Task DeleteAsync()
        {
            // Stop any ongoing tracking tasks
            await StopTrackingAsync();

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
            CoreDI.Logger.Info($"> Update started [{OriginalName}, {Url}]");
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
                DI.Application.NotificationReceived(result.Error, this, NotificationType.Warning);
                return;
            }

            #region Record & Notify

            // If a product original name has changed...
            if (result.HasNewName)
            {
                // Initialize name history
                if (NameHistory == null)
                {
                    // Initialize with old name in the history
                    NameHistory = new List<KeyValuePair<string, DateTime>>
                    {
                        new KeyValuePair<string, DateTime>(OriginalName, Settings.Created)
                    };
                }

                // Append new name to history
                NameHistory.Add(new KeyValuePair<string, DateTime>(Product.Name, DateTime.Now));

                // Handle notification
                DI.Application.NotificationReceived(string.Format(Strings.NotificationNameChanged, Product.Name),
                    this, popToast: Settings.NotifyPriceChange);

                // Update name
                OriginalName = Product.Name;

                // Raise property changed events
                OnPropertyChanged(nameof(OriginalName));
                OnPropertyChanged(nameof(Name));
            }

            // If a product price has changed...
            if (result.HasNewPrice)
            {
                // Initialize price history
                if (PriceHistory == null)
                {
                    // Initialize with old price in the history
                    PriceHistory = new List<KeyValuePair<decimal, DateTime>>{
                        new KeyValuePair<decimal, DateTime>(PriceCurrent, Settings.Created)
                    };
                }

                // Append new price to history
                PriceHistory.Add(new KeyValuePair<decimal, DateTime>(Product.PriceInfo.Value, DateTime.Now));

                // Update chart
                SeriesViews.First().Values.Add(Product.PriceInfo.Value);
                Labels.Add(DateTime.Now.ToShortDateString());

                // Handle notification
                DI.Application.NotificationReceived(
                    Product.PriceInfo.Value > PriceCurrent
                    // If new price is higher than current price...
                    ? string.Format(Strings.NotificationPriceIncrease, Product.PriceInfo.CurrencyAmount)
                    // If new price is lower than current price...
                    : string.Format(Strings.NotificationPriceDecrease, Product.PriceInfo.CurrencyAmount),
                    this, popToast: Settings.NotifyPriceChange);

                // Update price
                PriceCurrent = Product.PriceInfo.Value;

                // Raise property changed events
                OnPropertyChanged(nameof(PriceCurrent));
                OnPropertyChanged(nameof(DisplayPrice));
                OnPropertyChanged(nameof(LowestPrice));
                OnPropertyChanged(nameof(HighestPrice));
            }

            #endregion

            // If product has changed...
            if (result.Changed)
            {
                // Save application state after update
                DI.Application.Save();
            }

            // Leave a log message
            CoreDI.Logger.Info($"> Update finished [{OriginalName}]");
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
