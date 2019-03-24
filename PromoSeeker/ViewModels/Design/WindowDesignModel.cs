using System.Collections.ObjectModel;

namespace PromoSeeker
{
    /// <summary>
    /// The design-time data model for a <see cref="WindowViewModel"/>.
    /// </summary>
    public class WindowDesignModel : WindowViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model.
        /// </summary>
        public static WindowDesignModel Instance = new WindowDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public WindowDesignModel() : base(null)
        {
            // Populate tracked products collection
            Products = new ObservableCollection<ProductViewModel>
            {
                new ProductViewModel
                {
                    Name = "Test Product",
                    PriceCurrent = 1337
                },
                new ProductViewModel
                {
                    Name = "Test Product",
                    PriceCurrent = 1337
                },
                new ProductViewModel
                {
                    Name = "Test Product",
                    PriceCurrent = 1337
                },
            };
        }

        #endregion
    }
}
