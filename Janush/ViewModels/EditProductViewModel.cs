using Janush.Core;
using Janush.Core.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Janush
{
    /// <summary>
    /// The view model for <see cref="ProductEditPageControl"/>.
    /// </summary>
    public class EditProductViewModel : BaseViewModel, IWindowViewModel, IDataErrorInfo
    {
        #region Private Members

        /// <summary>
        /// Product URL.
        /// </summary>
        private string _url;

        /// <summary>
        /// If we are currently adding a product.
        /// </summary>
        private bool _isBusy;

        /// <summary>
        /// Whether the step two aka. product review overlay is visible.
        /// </summary>
        private bool _stepTwoVisible;

        /// <summary>
        /// The view model to edit.
        /// </summary>
        private ProductViewModel _targetViewModel;

        #endregion

        #region Public Properties

        /// <summary>
        /// The product detected name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Product full URL.
        /// </summary>
        public string Url
        {
            get => _url;
            set
            {
                // Update value
                _url = value;

                // Raise property changed event
                OnPropertyChanged(nameof(Url));
            }
        }

        /// <summary>
        /// The price selector 
        /// </summary>
        public string PriceSelector { get; set; }

        /// <summary>
        /// The regions to select the currency from.
        /// 
        /// TODO: Should we group the currencies by, for example
        /// the ISOCurrencySymbol to prevent duplicate currencies in the combo box
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
        /// Whether the step two aka. product review overlay is visible.
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
        /// The product to be updated;
        /// </summary>
        public ProductViewModel TargetViewModel
        {
            get => _targetViewModel;
            set
            {
                // Update value
                _targetViewModel = value;

                // Bind values
                if (value != null)
                {
                    Name = value.Name;
                    Url = value.Url.ToString();
                    UserRegion = value.CultureRegion;
                }
            }
        }

        /// <summary>
        /// The product price selected by the user, if product had more than one price detected.
        /// </summary>
        public PriceInfo SelectedPrice { get; set; }

        /// <summary>
        /// If the product has several price sources detected.
        /// </summary>
        public bool HasSeveralPrices => TargetViewModel.Product.DetectedPrices.Count > 1;

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
        /// The command to submit the changes.
        /// </summary>
        public ICommand SubmitCommand { get; set; }

        #region Validation

        private string _error = string.Empty;

        /// <summary>
        /// An error message indicating what is wrong with this object. The default is an
        /// empty string ("").
        /// </summary>
        public string Error
        {
            get => _error;
            private set
            {
                _error = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public Dictionary<string, string> Errors { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Verifies whether all properties in the object are passing the validation.
        /// </summary>
        public bool IsValid => !Errors.Any();

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="columnName">The property name.</param>
        /// <returns>The error message for the property. The default is an empty string ("").</returns>
        public string this[string columnName]
        {
            get
            {
                // Name validation
                if (columnName == nameof(Name))
                {
                    // If empty...
                    if (string.IsNullOrWhiteSpace(Name))
                    {
                        return Error = Errors[columnName] = string.Format(ValidationStrings.ErrorFieldEmpty, "name");
                    }

                    // If name is taken...
                    if (DI.Application.Products.Where(_ => _ != TargetViewModel).Any(_ => _.Name.Equals(Name)))
                    {
                        return Error = Errors[columnName] = ValidationStrings.ErrorNameTaken;
                    }
                }

                // URL validation
                if (columnName == nameof(Url))
                {
                    // If empty...
                    if (string.IsNullOrWhiteSpace(Url))
                    {
                        return Error = Errors[columnName] = string.Format(ValidationStrings.ErrorFieldEmpty, "URL");
                    }

                    // Validate the URL
                    var result = Uri.TryCreate(Url, UriKind.Absolute, out var uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                    // If unable to parse URL...
                    if (!result)
                    {
                        return Error = Errors[columnName] = ValidationStrings.ErrorInvalidURL;
                    }

                    // If the product was already added...
                    if (DI.Application.Products.Where(_ => _ != TargetViewModel).Any(_ => _.Url.Equals(Url)))
                    {
                        return Error = Errors[columnName] = ValidationStrings.ErrorProductExists;
                    }
                }

                // No error
                Errors.Remove(columnName);
                Error = string.Empty;
                return Error;
            }
        }


        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public EditProductViewModel()
        {
            // Create commands
            OpenCommand = new RelayCommand(Open);
            CloseCommand = new RelayCommand(Close);
            SubmitCommand = new RelayCommand(OnSubmit);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens a window.
        /// </summary>
        public void Open() => DI.Application.CurrentProductEdit = this;

        /// <summary>
        /// Closes a window.
        /// </summary>
        public void Close() => DI.Application.CurrentProductEdit = null;

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

            // Flag as busy
            IsBusy = true;

            // Create product
            var product = new Product(Url);

            // Load product and get the result
            var result = await Task.Run(product.LoadAsync);

            // If product was successfully loaded...
            if (result.Success)
            {
                // If several prices have been detected...
                if (product.DetectedPrices?.Count > 1)
                {
                    // Inform the user about several prices
                    await DI.UIManager.ShowMessageDialogBoxAsync(new MessageDialogBoxViewModel
                    {
                        Type = DialogBoxType.Information,
                        Title = "We are not sure about the price!",
                        Message = "We wasn't able to detect a certain product price. Several prices have been collected and you will be able to select a valid price in the next step."
                    });
                }


                // Create product region info
                UserRegion = new RegionInfo(TargetViewModel.Culture.Name);

                // Raise property changed event
                OnPropertyChanged(nameof(TargetViewModel));
                OnPropertyChanged(nameof(HasSeveralPrices));
                OnPropertyChanged(nameof(UserRegion));

                // Show confirmation page
                StepTwo = true;
            }
            // Otherwise...
            else
            {
                // Cleanup
                product.Dispose();

                // Show error message to the user
                _ = DI.UIManager.ShowMessageDialogBoxAsync(new MessageDialogBoxViewModel
                {
                    Type = DialogBoxType.Error,
                    Message = result.Error.ToDisplayString()
                });
            }

            // Unset busy flag
            IsBusy = false;
        }

        /// <summary>
        /// Adds a recently loaded product to the tracker.
        /// </summary>
        private void OnSubmit()
        {
            // If we have no product...
            if (TargetViewModel == null)
            {
                return;
            }

            // If fails validation...
            if (!IsValid)
            {
                return;
            }

            try
            {
                // Create new culture
                var newCulture = CultureInfo.CreateSpecificCulture(UserRegion.Name);

                // Update props
                TargetViewModel.DisplayName = Name;

                // If URL has changed...
                if (!TargetViewModel.Url.Equals(Url))
                {
                    // TODO: reverify product
                    Debugger.Break();
                }

                TargetViewModel.Url = new Uri(Url);
                TargetViewModel.Culture = newCulture;

                // Update local data store
                DI.Application.Save();
            }
            finally
            {
                // Close window
                Close();

                // Show main window
                Application.Current.MainWindow?.Show();
            }

            // TODO: ensure price selector
        }

        #endregion
    }
}
