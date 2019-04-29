using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using System;
using System.Threading.Tasks;

namespace PromoSeeker.Core
{
    /// <summary>
    /// A website document loader utility class.
    /// </summary>
    public sealed class WebLoader
    {
        #region Public Properties

        /// <summary>
        /// A requester to be used for the HTTP requests.
        /// </summary>
        public static IRequester Requester = CreateRequester();

        /// <summary>
        /// The configuration to be used in the context.
        /// </summary>
        public static IConfiguration Configuration = AngleSharp.Configuration.Default
                .WithDefaultLoader()
                .With(Requester);

        #endregion

        /// <summary>
        /// Asynchronously loads a document from the provider <paramref name="url"/> address.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>The loaded document instance.</returns>
        public static async Task<IDocument> LoadAsync(string url)
        {
            // Create a context for the request using our configuration
            var context = BrowsingContext.New(Configuration);

            // Return the document once loaded
            return await context.OpenAsync(url);
        }

        /// <summary>
        /// Asynchronously loads a document from the provider <paramref name="url"/> address and waits until stylesheets and scripts are ready.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>The loaded document instance.</returns>
        public static async Task<IDocument> LoadReadyAsync(string url)
        {
            // Load document
            var document = await LoadAsync(url);

            // Wait until the document is ready
            await document.WaitForReadyAsync();

            // Return the document
            return document;
        }

        #region Private Methods

        /// <summary>
        /// Creates a requester to be used for the HTTP requests.
        /// </summary>
        /// <returns></returns>
        private static IRequester CreateRequester()
        {
            var requester = new DefaultHttpRequester()
            {
                Timeout = TimeSpan.FromSeconds(10),
            };

            requester.Headers[HeaderNames.UserAgent] = Consts.USER_AGENT;
            requester.Headers["dnt"] = "1";

            return requester;
        }

        #endregion
    }
}
