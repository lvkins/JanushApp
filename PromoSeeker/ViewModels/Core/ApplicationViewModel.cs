﻿using System;
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
        /// <param name="productSetting">The product settings object.</param>
        public void AddProduct(ProductSettings productSetting)
        {
            // Add the product to the collection
            Products.Add(new ProductViewModel(productSetting));

            // Store the product to the settings file
            DI.SettingsReader.Settings.Products.Add(productSetting);
            DI.SettingsReader.Save();
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
                products.OrderBy(_ => !_.Tracked).ToList()
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
                DI.UIManager.ShowMessageBoxAsync(new MessageDialogViewModel
                {
                    Type = DialogBoxType.Error,
                    Message = "Application state wasn't loaded properly, please ensure your settings file is corrupted.", // TODO: localize me
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
            Debug.WriteLine($"Save application state {System.Threading.Thread.CurrentThread.ManagedThreadId}");

            // Update products
            DI.SettingsReader.Settings.Products = Products.Select(_ => new ProductSettings
            {
                Url = _.Url,
                Name = _.Name,
                Price = _.PriceCurrent,
                Culture = _.Culture,
                DisplayName = !_.Name.Equals(_.DisplayName, StringComparison.InvariantCulture) ? _.DisplayName : null,
                Tracked = _.Tracked,
                LastChecked = _.LastCheck,
                Created = _.DateAdded,
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
                window.Owner = Application.Current.MainWindow;
                window.DataContext = viewModel;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.ResizeMode = ResizeMode.NoResize;

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
