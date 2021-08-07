using AngleSharp;
using PuppeteerSharp;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Janush.Core
{
    /// <summary>
    /// A website document loader utility class.
    /// </summary>
    public sealed class Browser : IAsyncDisposable
    {
        #region Public Properties

        /// <summary>
        /// A browser instance
        /// </summary>
        public PuppeteerSharp.Browser Instance { get; set; }

        /// <summary>
        /// The configuration to be used in the context.
        /// </summary>
        public IConfiguration Configuration { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Browser()
        {
        }

        #endregion

        #region Public Methods

        public async Task InitializeAsync()
        {
            Debug.WriteLine("Initializing browser...");
            var browserFetcher = new BrowserFetcher();
            Debug.WriteLine("Downloading browser...");
            await browserFetcher.DownloadAsync();
            Debug.WriteLine("Browser downloaded...");
            Instance = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = false });
        }

        /// <summary>
        /// Cleans up resources used
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await Instance.DisposeAsync();
        }

        #endregion

    }
}
