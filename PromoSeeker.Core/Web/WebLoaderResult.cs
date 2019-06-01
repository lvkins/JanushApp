using AngleSharp.Dom;

namespace PromoSeeker.Core
{
    /// <summary>
    /// A class containing the result of a single website load.
    /// </summary>
    public class WebLoaderResult
    {
        /// <summary>
        /// If the redirection status code was encountered during the request route.
        /// </summary>
        public bool Redirected { get; set; }

        /// <summary>
        /// The load result document.
        /// </summary>
        public IDocument Document { get; set; }
    }
}
