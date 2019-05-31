using PromoSeeker.Core.Localization;
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
        /// A redirection was encountered while loading.
        /// Product could be moved or is no longer available.
        /// </summary>
        Redirected,

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
            switch (type)
            {
                case ProductLoadResultErrorType.NoResponse: return Strings.ErrorProductLoadNoResponse;
                case ProductLoadResultErrorType.InvalidResponse: return Strings.ErrorProductLoadInvalidResponse;
                case ProductLoadResultErrorType.Redirected: return Strings.ErrorProductLoadRedirected;
                case ProductLoadResultErrorType.ProductNotDetected: return Strings.ErrorProductLoadNotDetected;
                case ProductLoadResultErrorType.ProductParamNotFound: return Strings.ErrorProductLoadNotFound;
                case ProductLoadResultErrorType.ProductUnknownPrice: return Strings.ErrorProductLoadUnknownPrice;
                case ProductLoadResultErrorType.ProductUnknownName: return Strings.ErrorProductLoadUnknownName;
                case ProductLoadResultErrorType.ProductUnknownCulture: return Strings.ErrorProductLoadUnknownCulture;
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
        }
    }
}
