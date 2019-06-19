using System;
using System.Collections.ObjectModel;

namespace Janush
{
    /// <summary>
    /// The design-time data model for a <see cref="ApplicationDesignModel"/>.
    /// </summary>
    public class ApplicationDesignModel : ApplicationViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model.
        /// </summary>
        public static ApplicationDesignModel Instance = new ApplicationDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApplicationDesignModel()
        {
            // Create products
            Products = new ObservableCollection<ProductViewModel>
            {
                new ProductViewModel
                {
                    DisplayName = "Test Product",
                    PriceCurrent = 100.0M,
                    Tracked = true,
                },
                new ProductViewModel
                {
                    DisplayName = "Another Product",
                    PriceCurrent = 100.50M,
                    Tracked = true,
                    LastCheck = DateTime.Now.Subtract(TimeSpan.FromMinutes(30))
                },
                new ProductViewModel
                {
                    DisplayName = "Disabled Product",
                    PriceCurrent = 50.00M
                },
                new ProductViewModel
                {
                    DisplayName = "Disabled Product",
                    PriceCurrent = 50.00M
                },
                new ProductViewModel
                {
                    DisplayName = "Disabled Product",
                    PriceCurrent = 50.00M
                },
                new ProductViewModel
                {
                    DisplayName = "Disabled Product",
                    PriceCurrent = 50.00M
                },
                new ProductViewModel
                {
                    DisplayName = "Disabled Product",
                    PriceCurrent = 50.00M
                },
                new ProductViewModel
                {
                    DisplayName = "Disabled Product",
                    PriceCurrent = 50.00M
                },
                new ProductViewModel
                {
                    DisplayName = "Disabled Product",
                    PriceCurrent = 50.00M
                }
};
        }

        #endregion
    }
}
