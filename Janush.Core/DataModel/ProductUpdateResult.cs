namespace Janush.Core
{
    /// <summary>
    /// A class that represents updated product result.
    /// </summary>
    public class ProductUpdateResult
    {
        /// <summary>
        /// An update error, if any.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Whether the update was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Indicates whether the product has a new name.
        /// </summary>
        public bool HasNewName { get; set; }

        /// <summary>
        /// Indicates whether the product has a new price.
        /// </summary>
        public bool HasNewPrice { get; set; }

        /// <summary>
        /// Whether any of the product property (price, name, ...) has changed.
        /// </summary>
        public bool Changed => HasNewName || HasNewPrice;
    }
}
