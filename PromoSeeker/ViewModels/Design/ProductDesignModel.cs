namespace PromoSeeker
{
    /// <summary>
    /// The design-time data model for a <see cref="ProductViewModel"/>.
    /// </summary>
    public class ProductDesignModel : ProductViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model.
        /// </summary>
        public static ProductDesignModel Instance = new ProductDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProductDesignModel()
        {
            // Set design time properties
            DisplayName = "My very cool design time product!";
            PriceCurrent = 100.50M;
        }

        #endregion
    }
}
