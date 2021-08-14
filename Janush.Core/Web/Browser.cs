using AngleSharp;
using PuppeteerSharp;
using System;
using System.Diagnostics;
using System.Linq;
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
            if (Instance != null)
            {
                // Dispose existing instance
                await Instance.DisposeAsync();
                Instance = null;
            }

            Debug.WriteLine("Initializing browser...");
            var browserFetcher = new BrowserFetcher();
            Debug.WriteLine("Downloading browser...");
            await browserFetcher.DownloadAsync();
            Debug.WriteLine("Browser downloaded...");
            Instance = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = false });
            Instance.Closed += Instance_ClosedAsync;
        }

        /// <summary>
        /// Gets a test page used for temporary products - this is initially 'about:blank' browser tab page.
        /// </summary>
        /// <returns></returns>
        public async Task<Page> TestPageAsync(bool blockHeavyRequests = true)
        {
            // Get initial page
            var page = (await Instance.PagesAsync())[0];

            // Set page request interception 
            await page.SetRequestInterceptionAsync(blockHeavyRequests);
            page.Request -= Page_RequestBlockHeavyResources;

            if (blockHeavyRequests)
            {
                // Subscribe to request blocking event
                page.Request += Page_RequestBlockHeavyResources;
            }

            return page;
        }

        /// <summary>
        /// Creates a new browser tab.
        /// </summary>
        /// <param name="blockHeavyRequests">Whether to block heavy resource requests like images, spreadsheets, fonts.</param>
        /// <returns>An usable browser tab page.</returns>
        public async Task<Page> CreatePageAsync(bool blockHeavyRequests = true)
        {
            // Get current pages
            var pages = await Instance.PagesAsync();
            // Find unused page
            var page = Enumerable.FirstOrDefault(pages, _ => _.Url == "about:blank");

            // Subscribe to request event to block redundant resources 
            // and speed up the page load
            if (blockHeavyRequests)
            {
                await page.SetRequestInterceptionAsync(true);
                page.Request += Page_RequestBlockHeavyResources;
            }

            return page;
        }

        /// <summary>
        /// Cleans up resources used
        /// </summary>
        public async ValueTask DisposeAsync() => await Instance.DisposeAsync();

        #endregion

        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Instance_ClosedAsync(object sender, EventArgs e) => await InitializeAsync();

        /// <summary>
        /// Prevents page from downloading heavy resources like images, stylesheets or fonts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_RequestBlockHeavyResources(object sender, RequestEventArgs e)
        {
            if (e.Request.ResourceType == ResourceType.StyleSheet ||
                e.Request.ResourceType == ResourceType.Font ||
                e.Request.ResourceType == ResourceType.Image)
            {
                e.Request.AbortAsync();
            }
            else
            {
                e.Request.ContinueAsync();
            }
        }

        #endregion
    }
}
