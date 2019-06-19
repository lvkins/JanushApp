using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using System;
using System.Threading.Tasks;

namespace Janush.Core
{
    /// <summary>
    /// A website document loader utility class.
    /// </summary>
    public sealed class WebLoader : IWebLoader
    {
        #region Private Members

        /// <summary>
        /// A context instance for the requests.
        /// </summary>
        private readonly IBrowsingContext _context;

        #endregion

        #region Public Properties

        /// <summary>
        /// A requester to be used for the HTTP requests.
        /// </summary>
        public IRequester Requester { get; }

        /// <summary>
        /// The configuration to be used in the context.
        /// </summary>
        public IConfiguration Configuration { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public WebLoader()
        {
            // Create a requester
            Requester = CreateRequester();

            // Create a configuration for the browsing context
            Configuration = AngleSharp.Configuration.Default
                .With(Requester)
                .WithDefaultLoader(new LoaderOptions
                {
                    IsResourceLoadingEnabled = false,
                })
                // Enable XPath queries in QuerySelector method (*[xpath>'//li[2]'])
                .WithXPath();

            // Create a browsing context
            _context = BrowsingContext.New(Configuration);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Asynchronously loads a document from the provider <paramref name="url"/> address.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>The loaded document instance.</returns>
        public async Task<WebLoaderResult> LoadAsync(string url)
        {
            // If the route was redirected
            var redirected = false;

            // Prepare the requester callback
            DomEventHandler requesterCallback = (object s, AngleSharp.Dom.Events.Event e)
                => Requester_Requested(e as AngleSharp.Dom.Events.RequestEvent, ref redirected);

            // Hook into requested event to monitor a single request in the requester
            Requester.Requested += requesterCallback;

            // Create the result
            var result = new WebLoaderResult
            {
                Redirected = redirected,
                Document = await _context.OpenAsync(url),
            };

            // Once loaded, release hook
            Requester.Requested -= requesterCallback;

            // Return the document once loaded
            return result;
        }

        /// <summary>
        /// Asynchronously loads a document from the provider <paramref name="url"/> address and waits until stylesheets and scripts are ready.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>The loaded document instance.</returns>
        public async Task<WebLoaderResult> LoadReadyAsync(string url)
        {
            // Get load result
            var result = await LoadAsync(url);

            // Wait until the document is ready
            await result.Document.WaitForReadyAsync();

            // Return the document
            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a requester to be used for the HTTP requests.
        /// </summary>
        /// <returns></returns>
        private IRequester CreateRequester()
        {
            // Create default requester
            var requester = new DefaultHttpRequester()
            {
                Timeout = TimeSpan.FromSeconds(10),
            };

            // Set headers
            requester.Headers[HeaderNames.UserAgent] = Consts.USER_AGENT;
            requester.Headers["dnt"] = "1";

            return requester;
        }

        /// <summary>
        /// A requester callback to capture each request response.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="redirected"></param>
        private static void Requester_Requested(AngleSharp.Dom.Events.RequestEvent e, ref bool redirected)
        {
            // If we have request event and redirect code...
            if ((int)e.Response.StatusCode >= 300 &&
                (int)e.Response.StatusCode < 399)
            {
                // Flag as redirected
                redirected = true;
            }
        }

        #endregion
    }
}
