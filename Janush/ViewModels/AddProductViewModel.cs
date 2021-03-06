﻿// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

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
    /// The view model for <see cref="AddProductWindow"/>.
    /// </summary>
    public class AddProductViewModel : BaseViewModel, IWindowViewModel, IDataErrorInfo
    {
        #region Private Members

        /// <summary>
        /// Product full URL.
        /// </summary>
        private string _url;

        /// <summary>
        /// Product full URL.
        /// </summary>
        private string _manUrl;

        /// <summary>
        /// If we are currently adding a product.
        /// </summary>
        private bool _isBusy;

        /// <summary>
        /// Whether the step two aka. product review overlay is visible.
        /// </summary>
        private bool _stepTwoVisible;

        /// <summary>
        /// The currently selected add type option index.
        /// </summary>
        private int _selectedTabIndex;

        #endregion

        #region Public Properties

        /// <summary>
        /// The currently selected add type option index.
        /// </summary>
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                // Update value
                _selectedTabIndex = value;

                // Raise property changed events to re-validate fields
                // affected by changing the tab
                OnPropertyChanged(nameof(Url));
                OnPropertyChanged(nameof(ManualUrl));
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(PriceSelector));
            }
        }

        /// <summary>
        /// The product detected name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The product display name.
        /// </summary>
        public string DisplayName { get; set; }

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
        /// The product URL.
        /// </summary>
        public string ManualUrl
        {
            get => _manUrl;
            set
            {
                // Update value
                _manUrl = value;

                // Raise property changed event
                OnPropertyChanged(nameof(ManualUrl));
            }
        }

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
        /// The product to be added;
        /// </summary>
        public Product Product { get; private set; }

        /// <summary>
        /// The product price selected by the user, if product had more than one price detected.
        /// </summary>
        public PriceInfo SelectedPrice { get; set; }

        /// <summary>
        /// If the product has several price sources detected.
        /// </summary>
        public bool HasSeveralPrices => Product?.DetectedPrices.Count > 1;

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

        /// <summary>
        /// The command to discard the last loaded product.
        /// </summary>
        public ICommand DiscardProductCommand { get; set; }

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
                // URL validation
                if (columnName == nameof(Url) || columnName == nameof(ManualUrl))
                {
                    if (SelectedTabIndex != 0 && columnName == nameof(Url))
                    {
                        Errors.Remove(columnName);
                        return string.Empty;
                    }
                    else if (SelectedTabIndex != 1 && columnName == nameof(ManualUrl))
                    {
                        Errors.Remove(columnName);
                        return string.Empty;
                    }

                    var urlProp = SelectedTabIndex == 0 ? Url : ManualUrl;

                    // If empty...
                    if (string.IsNullOrWhiteSpace(urlProp))
                    {
                        return Error = Errors[columnName] = string.Format(ValidationStrings.ErrorFieldEmpty, "URL");
                    }

                    // Validate the URL
                    var result = Uri.TryCreate(urlProp, UriKind.Absolute, out var uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                    // If unable to parse URL...
                    if (!result)
                    {
                        return Error = Errors[columnName] = ValidationStrings.ErrorInvalidURL;
                    }

                    // If the product was already added...
                    if (DI.Application.Products.Any(_ => _.Url.Equals(urlProp)))
                    {
                        return Error = Errors[columnName] = ValidationStrings.ErrorProductExists;
                    }
                }

                // If selected "Add Manually" tab...
                if (SelectedTabIndex == 1)
                {
                    if ((columnName == nameof(Name)) && string.IsNullOrWhiteSpace(Name))
                    {
                        return Error = Errors[columnName] = string.Format(ValidationStrings.ErrorFieldEmpty, "name");
                    }
                    if ((columnName == nameof(PriceSelector)) && string.IsNullOrWhiteSpace(PriceSelector))
                    {
                        return Error = Errors[columnName] = string.Format(ValidationStrings.ErrorFieldEmpty, "price selector");
                    }
                }

                // TODO: validate confirmation controls (ProductReviewControl)

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
        public AddProductViewModel()
        {
            // Create commands
            OpenCommand = new RelayCommand(Open);
            CloseCommand = new RelayCommand(Close);
            AddAutoCommand = new RelayCommand(async () => await AddAutoProductAsync());
            AddManuallyCommand = new RelayCommand(async () => await AddProductManuallyAsync());
            ConfirmProductCommand = new RelayCommand(ConfirmProduct);
            DiscardProductCommand = new RelayCommand(Cleanup);
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

                // Store product for confirmation
                Product = product;

                // Create product region info
                UserRegion = new RegionInfo(Product.Culture.Name);

                // Raise property changed event
                OnPropertyChanged(nameof(Product));
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

            // Set busy flag
            IsBusy = true;

            // Create user selected culture
            var formatProvider = new CultureInfo(UserRegion.Name);

            // Create product
            var product = new Product(new ProductDataModel
            {
                Name = Name,
                Url = new Uri(ManualUrl),
                AutoDetect = false,
                Culture = formatProvider,
                Price = new PriceInfo(default, formatProvider)
                {
                    PriceXPathOrSelector = PriceSelector
                }
            });

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
        private void ConfirmProduct()
        {
            // If we have no product stored...
            if (Product == null)
            {
                return;
            }

            // If user had to select a valid price...
            if (Product.DetectedPrices?.Count > 1)
            {
                // If no price was selected...
                if (SelectedPrice == null)
                {
                    return;
                }

                // Set selected price to be tracked
                Product.SetTrackingPrice(SelectedPrice);
            }

            // Create setting object
            var productSetting = new ProductDataModel
            {
                Url = new Uri(Product.Url),// new Uri(Product.Url.Replace("://www.", "://")),
                Name = Product.Name,
                DisplayName = DisplayName,
                Price = Product.PriceInfo,
                Culture = Product.Culture,
                LastChecked = DateTime.Now,
                AutoDetect = Product.IsAutoDetect,
            };

            // Add product
            DI.Application.AddProduct(productSetting);

            // Close product review overlay and cleanup user input
            Cleanup();

            // Close window
            Close();

            // Show main window
            Application.Current.MainWindow?.Show();
        }

        /// <summary>
        /// Clean up a recently loaded product.
        /// </summary>
        private void Cleanup()
        {
            StepTwo = false;
            Product = null;
            Name = default;
            Url = default;
            ManualUrl = default;
            PriceSelector = default;
            SelectedPrice = default;
            Status = default;
        }

        #endregion
    }
}
