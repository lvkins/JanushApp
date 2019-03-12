using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace PromoSeeker.Core
{
    /// <summary>
    /// A wrapper around the <see cref="HtmlWeb"/> class.
    /// </summary>
    public class HtmlRequest
    {
        #region Private Members
        
        /// <summary>
        /// The web response awaiting task.
        /// </summary>
        private readonly TaskCompletionSource<HttpWebResponse> mResponseTsk
            = new TaskCompletionSource<HttpWebResponse>();

        /// <summary>
        /// A HTML loader utility class. 
        /// </summary>
        private readonly HtmlWeb mHtmlWeb;

        #endregion

        #region Public Properties
        
        /// <summary>
        /// The complete, loaded HTML document.
        /// </summary>
        public HtmlAgilityPack.HtmlDocument Document { get; private set; }

        /// <summary>
        /// The HTTP web response object.
        /// </summary>
        public HttpWebResponse Response { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HtmlRequest()
        {
            // Create HtmlWeb with request options
            mHtmlWeb = new HtmlWeb()
            {
                UserAgent = Consts.USER_AGENT,
                AutoDetectEncoding = true,
                CaptureRedirect = false,
                UseCookies = false,
                UsingCache = false,
                UsingCacheIfExists = false,
                //BrowserTimeout = TimeSpan.FromSeconds(10),
                //BrowserDelay = TimeSpan.FromSeconds(1),
            };

            // Hook into pre-request event
            mHtmlWeb.PreRequest += (HttpWebRequest request) =>
            {
                // Add headers

                // Encoding
                request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip, deflate";

                // Language preference
                //request.Headers[HttpRequestHeader.AcceptLanguage] = "en-US";

                // Do not track
                request.Headers.Add("dnt", "1");

                // Set decompression type used which is required for some pages
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                //request.CookieContainer = new System.Net.CookieContainer();

                return true;
            };

            // Hook into post-request event
            mHtmlWeb.PostResponse += (HttpWebRequest request, HttpWebResponse response) =>
            {
                // Set response task result
                mResponseTsk.SetResult(response);
            };

            // SSL certificate error fix
            // https://stackoverflow.com/questions/2859790/the-request-was-aborted-could-not-create-ssl-tls-secure-channel
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        } 

        #endregion

        #region Public Methods

        public async Task LoadAsync(string url)
        {
            // TODO: handle exceptions

            // Store loaded document
            Document = mHtmlWeb.Load(url);

            // Store a response
            Response = await mResponseTsk.Task;
        }

        /*
         * NOTE: Requires Windows.Forms reference.
         *
         * [Obsolete("Use LoadAsync method")]
        public HtmlAgilityPack.HtmlDocument LoadWithBrowser(string url)
        {
            return mHtmlWeb.LoadFromBrowser(url, o =>
            {
                var webBrowser = (WebBrowser)o;

                // Await ready state completed
                if (webBrowser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Debug.WriteLine("Browser wait complete state.");
                    return false;
                }

                webBrowser.ScriptErrorsSuppressed = true;
                webBrowser.ScrollBarsEnabled = false;

                // Script injection
                //var elem = webBrowser.Document.CreateElement("script");
                //elem.SetAttribute("text", SCRIPT);

                //var body = webBrowser.Document.Body;

                // Insert the script
                //body.AppendChild(elem);

                // Invoke the script
                //webBrowser.Document.InvokeScript("pseeker__setSizes");
                

                return true;
            });
        }
        */

        #endregion
    }
}
