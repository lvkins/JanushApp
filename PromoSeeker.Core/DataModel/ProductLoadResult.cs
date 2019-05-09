namespace PromoSeeker.Core
{
    /// <summary>
    /// A class that represents product load result.
    /// </summary>
    public class ProductLoadResult
    {
        /// <summary>
        /// If the product load is successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The result error type.
        /// </summary>
        public ProductLoadResultErrorType Error { get; set; }
    }
}
