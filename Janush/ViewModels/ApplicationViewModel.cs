// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

using Janush.Core;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace Janush
{
    /// <summary>
    /// The application main view model.
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// Whether the internet connection is available.
        /// </summary>
        private bool _isOnline = true;

        /// <summary>
        /// The currently selected product in the list.
        /// </summary>
        private ProductViewModel _selectedProduct;

        /// <summary>
        /// If the notifications popup is currently visible.
        /// </summary>
        private bool _notificationsPopupVisible;

        /// <summary>
        /// A view model for the currently shown product details.
        /// </summary>
        private ProductViewModel _currentProductDetails;

        /// <summary>
        /// A view model for the currently editing product.
        /// </summary>
        private EditProductViewModel _currentProductEdit;

        #endregion

        #region Public Properties

        /// <summary>
        /// Whether the internet connection is available.
        /// </summary>
        public bool IsOnline
        {
            get => _isOnline;
            set
            {
                // Update value
                _isOnline = value;

                // Raise property changed
                OnPropertyChanged(nameof(IsOnline));
            }
        }

        /// <summary>
        /// The added products collection.
        /// </summary>
        public ObservableCollection<ProductViewModel> Products { get; set; }
            = new ObservableCollection<ProductViewModel>();

        /// <summary>
        /// The shop ordered by product sales count and overall product count.
        /// </summary>
        public TopSellerViewModel TopSeller { get; set; }

        /// <summary>
        /// The currently selected product in the overall product list.
        /// </summary>
        public ProductViewModel SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                // Update value
                _selectedProduct = value;

                // Raise property changed
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }

        /// <summary>
        /// A view model for the currently shown product details.
        /// </summary>
        public ProductViewModel CurrentProductDetails
        {
            get => _currentProductDetails;
            set
            {
                // Update value
                _currentProductDetails = value;

                // Raise property changed
                OnPropertyChanged(nameof(CurrentProductDetails));
                OnPropertyChanged(nameof(ProductDetailsPageVisible));
            }
        }

        /// <summary>
        /// A view model for the editing product.
        /// </summary>
        public EditProductViewModel CurrentProductEdit
        {
            get => _currentProductEdit;
            set
            {
                // Update value
                _currentProductEdit = value;

                // Raise property changed
                OnPropertyChanged(nameof(CurrentProductEdit));
                OnPropertyChanged(nameof(ProductEditPageVisible));
            }
        }

        #region Notifications

        /// <summary>
        /// If the notifications popup is currently visible.
        /// </summary>
        public bool NotificationsPopupVisible
        {
            get => _notificationsPopupVisible;
            set
            {
                // Update value
                _notificationsPopupVisible = value;

                // Raise property changed
                OnPropertyChanged(nameof(NotificationsPopupVisible));
                OnPropertyChanged(nameof(AnyPopupVisible));
            }
        }

        #endregion

        /// <summary>
        /// Defines whether any popup within the main window is currently visible and present. 
        /// </summary>
        public bool AnyPopupVisible => NotificationsPopupVisible;

        /// <summary>
        /// <see langword="true"/> if the product details page is shown.
        /// </summary>
        public bool ProductDetailsPageVisible => CurrentProductDetails != null;

        /// <summary>
        /// <see langword="true"/> if the product edit page is shown.
        /// </summary>
        public bool ProductEditPageVisible => CurrentProductEdit != null;

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a new product to track to the application and stores it in the settings file.
        /// </summary>
        /// <param name="settings">The product settings object.</param>
        public void AddProduct(ProductDataModel settings)
        {
            // Store the product to the settings file
            CoreDI.SettingsReader.Settings.Products.Add(settings);
            CoreDI.SettingsReader.Save();

            // Add the product to the collection
            Products.Add(new ProductViewModel(settings));
        }

        /// <summary>
        /// Deletes a single product from the application.
        /// </summary>
        /// <param name="productViewModel">The product to delete.</param>
        public void DeleteProduct(ProductViewModel productViewModel)
        {
            // Get this product in the settings
            var result = CoreDI.SettingsReader.Settings.Products.Where(_ => _.Url == productViewModel.Url).FirstOrDefault();

            // If product was found...
            if (result != null)
            {
                CoreDI.SettingsReader.Settings.Products.Remove(result);
                CoreDI.SettingsReader.Save();
            }

            // Close details page
            if (CurrentProductDetails == productViewModel)
            {
                CurrentProductDetails = null;
            }

            // Close edit page
            if (CurrentProductEdit?.TargetViewModel == productViewModel)
            {
                CurrentProductEdit = null;
            }

            // Deselect product
            if (SelectedProduct == productViewModel)
            {
                SelectedProduct = null;
            }

            // Cleanup
            productViewModel.Product?.Dispose();

            // Remove from the collection
            Products.Remove(productViewModel);
        }

        /// <summary>
        /// Loads the application content state.
        /// </summary>
        public async void LoadAsync()
        {
            try
            {
                // Initialize browser
                await CoreDI.Browser.InitializeAsync();
            }
            catch (Exception ex)
            {
                // Let developer know
                Debugger.Break();

                // Unable to load application 
                CoreDI.Logger.Fatal("Browser initialization failed");
                CoreDI.Logger.Exception(ex);
            }

            try
            {
                // Load user settings here to catch all the exceptions and handle corrupted
                // user settings in one place
                CoreDI.SettingsReader.Load();

                // Get the stored products list
                var products = CoreDI.SettingsReader.Settings.Products;

                // Iterate over products list
                products.OrderBy(_ => !_.Tracked)
                    .ToList()
                    // Add the product to the collection
                    .ForEach(product => Products.Add(new ProductViewModel(product)));

                // Set top shop
                UpdateTopShop();
            }
            catch (Exception ex)
            {
                // Let developer know
                Debugger.Break();

                // Unable to load application 
                CoreDI.Logger.Fatal("Settings load failed");
                CoreDI.Logger.Exception(ex);

                // Show error to the user
                _ = DI.UIManager.ShowMessageDialogBoxAsync(new MessageDialogBoxViewModel
                {
                    Type = DialogBoxType.Error,
                    Message = "Application state wasn't loaded properly, please ensure your settings file is not corrupted.", // TODO: localize me
                });

                // TODO: Ask user to restore backed up settings file if we have one
            }
        }

        /// <summary>
        /// Saves the application content state.
        /// </summary>
        public void Save()
        {
            // Write diagnostic info
            Debug.WriteLine($"Save application state");

            // Update products
            CoreDI.SettingsReader.Settings.Products = Products.Select(_ =>
            {
                // Update stored settings values with the values in view model
                _.Settings.Url = _.Url;
                _.Settings.Name = _.OriginalName;
                _.Settings.Price = _.Product.PriceInfo;
                _.Settings.Culture = _.Culture;
                _.Settings.DisplayName = _.DisplayName;
                _.Settings.Tracked = _.Tracked;
                _.Settings.NameHistory = _.NameHistory;
                _.Settings.PriceHistory = _.PriceHistory;
                _.Settings.LastChecked = _.LastCheck;
                _.Settings.Created = _.DateAdded;

                // Return updates settings
                return _.Settings;
            }).ToList();

            // Save settings
            CoreDI.SettingsReader.Save();
        }

        /// <summary>
        /// Updates the current top shop.
        /// </summary>
        public void UpdateTopShop()
        {
            // Update property
            TopSeller = Products
                     // Group product by host
                     .GroupBy(_ => new Uri(_.Url.GetLeftPart(UriPartial.Authority).Replace("www.", "")))
                     // Gather data
                     .Select(_ => new TopSellerViewModel
                     {
                         // Set shop URL
                         Url = _.Key,
                         // Set product count in the group
                         ProductCount = _.Count(),
                         // Set sales count relying on the price history
                         SaleCount = _.Where(p => p.PriceHistory != null)
                             .Select(p =>
                             {
                                 // Number of sales for product
                                 var sales = 0;

                                 p.PriceHistory.Aggregate((seed, next) =>
                                {
                                    // If next price is lower than current...
                                    if (next.Key < seed.Key)
                                    {
                                        // We have a sale
                                        sales++;
                                    }

                                    return next;
                                });

                                 return sales;
                             })
                             // Get total number of sales for products in this shop
                             .Sum()
                     })
                     // Order by sales count, ultimately by the product count in the shop
                     .OrderByDescending(s => s.SaleCount)
                     .ThenByDescending(s => s.ProductCount)
                     // Get top result
                     .FirstOrDefault();

            // Raise property changed
            OnPropertyChanged(nameof(TopSeller));
        }

        #region Window Handling

        /// <summary>
        /// Shows a children window to the user.
        /// </summary>
        /// <param name="onClose">The action to be executed when the window was closed.</param>
        /// <typeparam name="T">The window type to be created.</typeparam>
        public void ShowWindow<T>(BaseViewModel viewModel, Action onClose = null)
            where T : Window
        {
            // Attempt to find the window in the currently initialized windows in our application
            var window = Application.Current.Windows.OfType<T>().FirstOrDefault();

            // If none window was found...
            if (window == null)
            {
                // Create new window
                window = Activator.CreateInstance<T>();
                window.Title = $"{Consts.APP_TITLE} • {window.Title}";
                window.Owner = Application.Current.MainWindow;
                window.DataContext = viewModel;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                //window.ResizeMode = ResizeMode.NoResize;

                // If we have close callback...
                if (onClose != null)
                {
                    window.Closed += (s, e) => onClose();
                }
            }

            // Show
            window.ShowDialog();
        }

        /// <summary>
        /// Closes all windows of a given type.
        /// </summary>
        /// <typeparam name="T">The type of a window to be closed.</typeparam>
        public void CloseAllWindow<T>()
            where T : Window
        {
            Application.Current.Windows.OfType<T>().ToList().ForEach(_ => _.Close());
        }

        #endregion

        #endregion
    }
}
