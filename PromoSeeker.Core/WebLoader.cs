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

        /// <summary>
        /// If the redirection status code was encountered during the request route.
        /// </summary>
        public bool Redirected { get; private set; }

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

        /// <summary>
        /// Asynchronously loads a document from the provider <paramref name="url"/> address.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>The loaded document instance.</returns>
        public async Task<IDocument> LoadAsync(string url)
        {
            // Return the document once loaded
            return await _context.OpenAsync(url);
        }

        /// <summary>
        /// Asynchronously loads a document from the provider <paramref name="url"/> address and waits until stylesheets and scripts are ready.
        /// </summary>
        /// <param name="url"></param>
        /// <returns>The loaded document instance.</returns>
        public async Task<IDocument> LoadReadyAsync(string url)
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

            // Hook into requested event to monitor a single request in the requester
            requester.Requested += Requester_Requested;

            return requester;
        }

        /// <summary>
        /// A callback that is called when there was a request in the requester.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ev"></param>
        private void Requester_Requested(object sender, AngleSharp.Dom.Events.Event ev)
        {
            // If we have request event and redirect code...
            if (ev is AngleSharp.Dom.Events.RequestEvent req &&
                (int)req.Response.StatusCode >= 300 &&
                (int)req.Response.StatusCode < 399)
            {
                // Flag as redirected
                Redirected = true;
            }
        }

        #endregion
    }
}
