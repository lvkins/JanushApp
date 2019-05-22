using System;
using System.Collections.ObjectModel;

namespace PromoSeeker
{
    /// <summary>
    /// The design-time data model for a <see cref="SettingsViewModel"/>.
    /// </summary>
    public class SettingsDesignModel : SettingsViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model.
        /// </summary>
        public static SettingsDesignModel Instance = new SettingsDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SettingsDesignModel()
        {
            // Set properties
            EnableSoundNotification = true;
            CheckInterval = TimeSpan.FromMinutes(10);


            // Create a dummy product for the collection
            var dummyProduct = new ProductViewModel
            {
                DisplayName = "Sample Product"
            };

            // Fill the logs messages
            Logs = new ObservableCollection<Tuple<ProductViewModel, string, DateTime>>()
            {
                new Tuple<ProductViewModel, string, DateTime>(dummyProduct, "Hello World", DateTime.Now),
                new Tuple<ProductViewModel, string, DateTime>(dummyProduct, "Hello World", DateTime.Now),
                new Tuple<ProductViewModel, string, DateTime>(dummyProduct, "Hello World", DateTime.Now),
                new Tuple<ProductViewModel, string, DateTime>(dummyProduct, "Hello World", DateTime.Now),
                new Tuple<ProductViewModel, string, DateTime>(dummyProduct, "Hello World", DateTime.Now),
                new Tuple<ProductViewModel, string, DateTime>(dummyProduct, "Hello World", DateTime.Now),
                new Tuple<ProductViewModel, string, DateTime>(dummyProduct, "Hello World", DateTime.Now),
                new Tuple<ProductViewModel, string, DateTime>(dummyProduct, "Hello World", DateTime.Now),
                new Tuple<ProductViewModel, string, DateTime>(dummyProduct, "Hello World", DateTime.Now),
                new Tuple<ProductViewModel, string, DateTime>(dummyProduct, "Hello World", DateTime.Now),
                new Tuple<ProductViewModel, string, DateTime>(dummyProduct, "Hello World", DateTime.Now),
                new Tuple<ProductViewModel, string, DateTime>(dummyProduct, "Hello World", DateTime.Now),
                new Tuple<ProductViewModel, string, DateTime>(dummyProduct, "Hello World. This is a longer, detailed message for the log entry. I wonder if this would fit nicely into the cell.", DateTime.Today),
            };
        }

        #endregion
    }
}
