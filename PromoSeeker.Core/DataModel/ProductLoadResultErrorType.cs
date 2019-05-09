using System;

namespace PromoSeeker.Core
{
    /// <summary>
    /// The error type of product loading result.
    /// </summary>
    public enum ProductLoadResultErrorType
    {
        /// <summary>
        /// Lack of the response occurred.
        /// </summary>
        NoResponse,

        /// <summary>
        /// Invalid response occurred.
        /// </summary>
        InvalidResponse,

        /// <summary>
        /// Unable to detect product at the specified endpoint.
        /// </summary>
        ProductNotDetected,

        /// <summary>
        /// Unable to detect a valid product name.
        /// </summary>
        ProductUnknownName,

        /// <summary>
        /// Unable to detect a valid product price.
        /// </summary>
        ProductUnknownPrice,

        /// <summary>
        /// Unable to detect a valid product culture.
        /// </summary>
        ProductUnknownCulture,

        /// <summary>
        /// Product was detected, but we wasn't able to acquire some of it's parameters.
        /// </summary>
        ProductParamNotFound,
    }

    /// <summary>
    /// The extension methods for <see cref="ProductLoadResultErrorType"/>.
    /// </summary>
    public static class ProductLoadResultErrorTypeExtensions
    {
        /// <summary>
        /// Converts a error type to the display error string.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToDisplayString(this ProductLoadResultErrorType type)
        {
            // TODO: localize
            switch (type)
            {
                case ProductLoadResultErrorType.NoResponse:
                    return "No response received. Please ensure that the given URL is valid and the product website is currently reachable.";
                case ProductLoadResultErrorType.InvalidResponse:
                    return "Invalid response received. Please ensure that product website is currently reachable and not under maintenance work.";
                case ProductLoadResultErrorType.ProductNotDetected:
                    return "Sorry, we were unable to detect a valid product under given URL.";
                case ProductLoadResultErrorType.ProductParamNotFound:
                    return "Sorry, we were unable to detect the product properties.";
                case ProductLoadResultErrorType.ProductUnknownPrice:
                    return "Sorry, we were unable to detect a valid price for the product.";
                case ProductLoadResultErrorType.ProductUnknownName:
                    return "Sorry, we were unable to detect a valid name for the product.";
                case ProductLoadResultErrorType.ProductUnknownCulture:
                    return "Sorry, we were unable to detect a valid language for the product.";
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
