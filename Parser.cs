using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace PromoSeeker
{
    public class Parser : HtmlWeb
    {
        public static Parser Instance = new Parser();

        /// <summary>
        /// 
        /// </summary>
        private readonly string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 OPR/58.0.3135.65";

        /// <summary>
        /// The script to inject.
        /// </summary>
        private readonly string SCRIPT = @"
function pseeker__setSizes()
{
    var items = document.getElementsByTagName('*');
    var style;
    for (var i = 0; i < items.length; i++) {
        style = items[i].currentStyle || window.getComputedStyle(items[i], null);
        try {
            items[i].setAttribute('data-test', style.fontSize);
        } catch (err) {
            items[i].setAttribute('data-ps-err', err.message);
        }
    }
}
";

        public Parser()
        {
            UserAgent = USER_AGENT;
            AutoDetectEncoding = true;
            CaptureRedirect = false;
            UseCookies = false;
            UsingCache = false;
            UsingCacheIfExists = false;
            BrowserTimeout = TimeSpan.FromSeconds(10);
            BrowserDelay = TimeSpan.FromSeconds(1);

            // SSL certificate error fix
            // https://stackoverflow.com/questions/2859790/the-request-was-aborted-could-not-create-ssl-tls-secure-channel
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //BrowserDelay = TimeSpan.FromSeconds(1);

            // Hook into pre request delegate
            PreRequest += (HttpWebRequest request) =>
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

            PostResponse += (HttpWebRequest request, HttpWebResponse response) =>
            {
                ;
            };
        }

        public HtmlAgilityPack.HtmlDocument Parse(string url)
        {
            //var testFromWeb = Load(url);
            //testFromWeb.Save("testFromWeb.html");

            if (true)
            {
                // TODO: catch System.Net.WebException
                return Load(url);
            }

            return LoadFromBrowser(url, o =>
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

                var elem = webBrowser.Document.CreateElement("script");
                elem.SetAttribute("text", SCRIPT);

                var body = webBrowser.Document.Body;

                // Insert the script
                body.AppendChild(elem);

                // Invoke the script
                webBrowser.Document.InvokeScript("pseeker__setSizes");
                return true;
            });
        }
    }
}
