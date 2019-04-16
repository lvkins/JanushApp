using PromoSeeker.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PromoSeeker
{
    /// <summary>
    /// The view model for <see cref="AddProductWindow"/>.
    /// </summary>
    public class AddProductViewModel : BaseViewModel, IWindowViewModel, IDataErrorInfo
    {
        #region Private Members

        /// <summary>
        /// If we are currently adding a product.
        /// </summary>
        private bool _isBusy;

        /// <summary>
        /// Whether the step two overlay is visible.
        /// </summary>
        private bool _stepTwoVisible;

        #endregion

        #region Public Properties

        /// <summary>
        /// The product name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Product full URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The price selector 
        /// </summary>
        public string PriceSelector { get; set; }

        /// <summary>
        /// Any status message about adding a product that is displayed to the user.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The regions to select the currency from.
        /// 
        /// TODO: Should we group the currencies by, for example
        /// the ISOCurrencySymbol to prevent multiple currencies 
        /// and give user the ability to select a currency, rather 
        /// than the country?
        /// 
        /// </summary>
        public IEnumerable<RegionInfo> Regions { get; } = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            // Select the region info of the culture
            .Select(_ => new RegionInfo(_.Name))
            // Ignore multiple regions with the same id
            .GroupBy(_ => _.GeoId)
            .Select(_ => _.First())
            // Prevent same regions
            .Distinct()
            // Order ascending by region name
            .OrderBy(_ => _.DisplayName)
            .ToList();

        /// <summary>
        /// The currently selected region informations, defaults to the current user region.
        /// </summary>
        public RegionInfo UserRegion { get; set; } = new RegionInfo(CultureInfo.CurrentUICulture.Name); // SelectedItem Binding is not working when using RegionInfo.CurrentRegion;

        /// <summary>
        /// If we are currently adding a product.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                // Update value
                _isBusy = value;

                // Raise property changed event
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        /// <summary>
        /// Whether the step two overlay is visible.
        /// </summary>
        public bool StepTwo
        {
            get => _stepTwoVisible;
            set
            {
                // Update value
                _stepTwoVisible = value;

                // Raise property changed event
                OnPropertyChanged(nameof(StepTwo));
            }
        }

        /// <summary>
        /// The product to be added;
        /// </summary>
        public Product Product { get; private set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command to open the <see cref="AddProductWindow"/>.
        /// </summary>
        public ICommand OpenCommand { get; set; }

        /// <summary>
        /// The command to close the <see cref="AddProductWindow"/>.
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command to add the promotion automatically.
        /// </summary>
        public ICommand AddAutoCommand { get; set; }

        /// <summary>
        /// The command to add the promotion manually.
        /// </summary>
        public ICommand AddManuallyCommand { get; set; }

        /// <summary>
        /// The command to confirm loaded product and store it in the tracker.
        /// </summary>
        public ICommand ConfirmProductCommand { get; set; }

        #region Validation

        /// <summary>
        /// An error message indicating what is wrong with this object. The default is an
        /// empty string ("").
        /// </summary>
        public string Error => this[null];

        /// <summary>
        /// Verifies whether all properties in the object are passing the validation.
        /// </summary>
        public bool IsValid => string.IsNullOrEmpty(Error);

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="columnName">The property name.</param>
        /// <returns>The error message for the property. The default is an empty string ("").</returns>
        public string this[string columnName]
        {
            get
            {
                if (columnName == null || columnName == nameof(Url))
                {
                    // If empty...
                    if (string.IsNullOrWhiteSpace(Url))
                    {
                        return "Please specify the URL.";
                    }

                    // Validate the URL
                    var result = Uri.TryCreate(Url, UriKind.Absolute, out var uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                    // If unable to parse URL...
                    if (!result)
                    {
                        return "This is not a valid URL";
                    }
                }

                return string.Empty;
            }
        }


        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AddProductViewModel()
        {
            // Create commands
            OpenCommand = new RelayCommand(Open);
            CloseCommand = new RelayCommand(Close);
            AddAutoCommand = new RelayCommand(async () => await AddAutoProductAsync());
            AddManuallyCommand = new RelayCommand(async () => await AddProductManuallyAsync());
            ConfirmProductCommand = new RelayCommand(ConfirmProduct);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens a window.
        /// </summary>
        public void Open() => DI.Application.ShowWindow<AddProductWindow>(this);

        /// <summary>
        /// Closes a window.
        /// </summary>
        public void Close() => DI.Application.CloseAllWindow<AddProductWindow>();

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads a new product by specified <see cref="Url"/> by automatically detecting it's properties and shows a confirmation page.
        /// </summary>
        /// <returns></returns>
        private async Task AddAutoProductAsync()
        {
            // Double validate to ensure there are no errors
            if (!IsValid)
            {
                return;
            }

            // TODO: Check if the product was already added

            /*
             * 1. Collect required product parameters (name, price, culture info (currency)).
             * 2. Show results to the user in the inputs,
             * 3. If some parameters were incorrect, give the user right to correct them,
             * 4. Price correction can be made via the dropdown of already detected prices - or via user specified xpath selector.
             * 5. After product is set - verify one more time,
             * 6. Store product.
             */

            IsBusy = true;

            // Create product
            var product = new Product(Url);

            // Load product and get the result
            var result = await Task.Run(product.LoadAsync);

            // If product was successfully loaded...
            if (result.Success)
            {
                // Store product for confirmation
                Product = product;

                // Create product region info
                UserRegion = new RegionInfo(Product.Culture.Name);

                // Raise property changed event
                OnPropertyChanged(nameof(Product));

                // Show confirmation page
                StepTwo = true;
            }
            // Otherwise...
            else
            {
                // Show error message to the user
                _ = DI.UIManager.ShowMessageBoxAsync(new MessageDialogViewModel
                {
                    Type = DialogBoxType.Error,
                    Message = result.Error ?? result.ErrorType.ToString()
                });
            }

            IsBusy = false;
        }

        /// <summary>
        /// Loads a new product by manually user specified properties and shows a confirmation page.
        /// </summary>
        /// <returns></returns>
        private async Task AddProductManuallyAsync()
        {
            // Double validate to ensure there are no errors
            if (!IsValid)
            {
                return;
            }

            IsBusy = true;

            // Create product
            var product = new Product(Name, Url, PriceSelector, new CultureInfo(UserRegion.Name));

            // Load product and get the result
            var result = await Task.Run(product.LoadAsync);

            // If product was successfully loaded...
            if (result.Success)
            {
                // Store product for confirmation
                Product = product;

                // Raise property changed event
                OnPropertyChanged(nameof(Product));

                // Show confirmation page
                StepTwo = true;
            }
            // Otherwise...
            else
            {
                // Show error message to the user
                _ = DI.UIManager.ShowMessageBoxAsync(new MessageDialogViewModel
                {
                    Type = DialogBoxType.Error,
                    Message = result.Error ?? result.ErrorType.ToString()
                });
            }

            IsBusy = false;
        }

        /// <summary>
        /// Adds a new product to the tracker.
        /// </summary>
        private void ConfirmProduct()
        {
            // If we have no product stored...
            if (Product == null)
            {
                return;
            }

            // Add product
            DI.Application.Products.Add(new ProductViewModel
            {
                Product = Product,
                Name = Product.Name,
                Url = new Uri(Product.Url),
                DateAdded = DateTime.Now,
                PriceCurrent = Product.PriceInfo.Price.Decimal
            });

            // Close and cleanup user input
            StepTwo = false;
            Name = string.Empty;
            Url = string.Empty;
            PriceSelector = string.Empty;
            Status = string.Empty;
            Product = null;

            // Close window
            Close();

            // Show main window
            Application.Current.MainWindow?.Show();
        }

        #endregion
    }
}
