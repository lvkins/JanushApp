using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using PromoSeeker.Core;

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
        /// Whether the product is currently being updated.
        /// </summary>
        private bool _currentlyUpdating;

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
        public bool ShowOptionsPopupMenu { get; set; }

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
        public bool Tracked { get; private set; }

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
        public ICommand ExpandOptionsPopupCommand { get; set; }

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

            #region Create Commands

            LoadCommand = new RelayParamCommand((param) => Load((ProductSettings)param));
            NavigateCommand = new RelayCommand(() => Process.Start(Url.Scheme + "://" + Url.Host));
            ExpandOptionsPopupCommand = new RelayCommand(() =>
            {
                // Toggle value
                ShowOptionsPopupMenu = !ShowOptionsPopupMenu;

                // Raise property changed event
                OnPropertyChanged(nameof(ShowOptionsPopupMenu));
            });

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
            PriceCurrent = product.Price;
            Url = product.Url;
            Tracked = product.Tracked;

            // Create product instance
            Product = new Product(Url.ToString());
            // Subscribe to the events
            Product.Updating += Product_Updating;
            Product.Updated += Product_Updated;

            // If the product tracking is enabled...
            if (Tracked)
            {
                // Start tracking
                StartTracking();
            }
        }

        /// <summary>
        /// Starts the tracking task for this product.
        /// </summary>
        public void StartTracking()
        {
            // Start tracking the product
            Product.Track(Consts.PRODUCT_UPDATE_INTERVAL);
        }

        /// <summary>
        /// Disable this product from tracking.
        /// </summary>
        public void StopTracking()
        {
            // TODO:
            // Raise cancellation token
            // Set properties
        }

        /// <summary>
        /// Saves the product to the file.
        /// </summary>
        public void Save()
        {
            // TODO: Save to the file
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets called whenever the product is updating.
        /// </summary>
        private void Product_Updating()
        {
            // Flag we are updating
            CurrentlyUpdating = true;
            Console.WriteLine("update started");
        }

        /// <summary>
        /// Gets called whenever the product is updated.
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

            Console.WriteLine("update finished");
            Console.WriteLine(Product.Name);
        }


        #endregion
    }
}
