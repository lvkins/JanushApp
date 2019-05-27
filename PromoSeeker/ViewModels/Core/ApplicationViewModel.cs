using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using PromoSeeker.Core;

namespace PromoSeeker
{
    /// <summary>
    /// The application main view model.
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The added products collection.
        /// </summary>
        public ObservableCollection<ProductViewModel> Products { get; set; }
            = new ObservableCollection<ProductViewModel>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a new product to track to the application and stores it in the settings file.
        /// </summary>
        /// <param name="settings">The product settings object.</param>
        public void AddProduct(ProductSettings settings)
        {
            // Store the product to the settings file
            DI.SettingsReader.Settings.Products.Add(settings);
            DI.SettingsReader.Save();

            // Add the product to the collection
            Products.Add(new ProductViewModel(settings));
        }

        /// <summary>
        /// Deletes a single product from the application.
        /// </summary>
        /// <param name="product">The product to delete.</param>
        public void DeleteProduct(ProductViewModel product)
        {
            // Get this product in the settings
            var result = DI.SettingsReader.Settings.Products.Where(_ => _.Url == product.Url).FirstOrDefault();

            // If product was found...
            if (result != null)
            {
                DI.SettingsReader.Settings.Products.Remove(result);
                DI.SettingsReader.Save();
            }

            // Remove from the collection
            Products.Remove(product);
        }

        /// <summary>
        /// Loads the application content state.
        /// </summary>
        public void Load()
        {
            try
            {
                // Load user settings here to catch all the exceptions and handle corrupted
                // user settings in one place
                DI.SettingsReader.Load();

                // Get the stored products list
                var products = DI.SettingsReader.Settings.Products;

                // Iterate over products list
                products.OrderBy(_ => !_.Tracked)
                    .ToList()
                    .ForEach(product =>
                    {
                        // Add the product to the collection
                        Products.Add(new ProductViewModel(product));
                    });
            }
            catch (Exception ex)
            {
                // Let developer know
                Debugger.Break();

                // Unable to load application 
                DI.Logger.Fatal("Load failed");
                DI.Logger.Exception(ex);

                // Show error to the user
                DI.UIManager.ShowMessageDialogBoxAsync(new MessageDialogBoxViewModel
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
            DI.SettingsReader.Settings.Products = Products.Select(_ =>
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
            DI.SettingsReader.Save();
        }

        /// <summary>
        /// Shows a children window to the user.
        /// </summary>
        /// <param name="onClose">The action to be executed when the window was closed.</param>
        /// <typeparam name="T">The window type to be created.</typeparam>
        public void ShowWindow<T>(BaseViewModel viewModel, Action<object> onClose = null)
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
                    window.Closed += (s, e) => onClose(s);
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
    }
}
