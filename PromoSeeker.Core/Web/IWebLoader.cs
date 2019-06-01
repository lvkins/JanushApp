using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using System.Threading.Tasks;

namespace PromoSeeker.Core
{
    /// <summary>
    /// A website document loader utility.
    /// </summary>
    public interface IWebLoader
    {
        #region Properties

        /// <summary>
        /// A requester to be used for the HTTP requests.
        /// </summary>
        IRequester Requester { get; }

        /// <summary>
        /// The configuration to be used in the context.
        /// </summary>
        IConfiguration Configuration { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Asynchronously loads a document from the provider <paramref name="url"/> address.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>The loaded document instance.</returns>
        Task<WebLoaderResult> LoadAsync(string url);

        /// <summary>
        /// Asynchronously loads a document from the provider <paramref name="url"/> address and waits until stylesheets and scripts are ready.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>The loaded document instance.</returns>
        Task<WebLoaderResult> LoadReadyAsync(string url);

        #endregion
    }
}
