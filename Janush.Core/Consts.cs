using System;
using System.Collections.Generic;

namespace Janush.Core
{
    public static class Consts
    {
        /// <summary>
        /// The main application name.
        /// </summary>
        public const string APP_TITLE = "Janush";

        /// <summary>
        /// The fake user agent to use for the HTTP requests.
        /// </summary>
        public const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.88 Safari/537.36";

        /// <summary>
        /// The currency format specifier to be used across application;
        /// </summary>
        public const string CURRENCY_FORMAT = "C2";

        /// <summary>
        /// The product title maximum length.
        /// </summary>
        public const int PRODUCT_TITLE_MAX_LENGTH = 96;

        /// <summary>
        /// The number of latest logs messages to show in the log window.
        /// </summary>
        public const int LOGS_LIMIT = 100;

        /// <summary>
        /// The maximum amount of the notifications to maintain.
        /// </summary>
        public const int NOTIFICATION_MAX_COUNT = 50;

        /// <summary>
        /// The default time interval, product will updated within.
        /// </summary>
        public static readonly TimeSpan PRODUCT_UPDATE_INTERVAL = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Whether if the <see cref="PRODUCT_UPDATE_INTERVAL"/> should be randomized by adding a small random values.
        /// </summary>
        public const bool PRODUCT_UDPATE_INTERVAL_RANDOMIZE = true;

        /// <summary>
        /// The sources to look for the product title.
        /// </summary>
        public static readonly IDictionary<string, string> TITLE_SOURCES = new Dictionary<string, string>
        {
            {"//meta[@property='og:title']", "content"},
            {"//head/title", null},
        };

        /// <summary>
        /// The sources to look for the product price.
        /// </summary>
        public static readonly IDictionary<string, string> PRICE_SOURCES = new Dictionary<string, string>
        {
            // Meta tag attribute is the most trusted price source.
            // NOTE: Some of these should appear on meta tag, however
            //  seen sites declaring it on different tags, hence no 
            //  specific tag selector
            {"[itemprop='price']", "content"},
            {"[property='og:price:amount']", "content"},
            {"[property='product:price:amount']", "content"},
            {"[name='twitter:data1']", "content"}, // Twitter product card - price definition
        };

        /// <summary>
        /// The sources to look for the product price currency.
        /// </summary>
        public static readonly IDictionary<string, string> CURRENCY_SOURCES = new Dictionary<string, string>
        {
            // NOTE: Some of these should appear on meta tag, however
            //  seen sites declaring it on different tags, hence no 
            //  specific tag selector
            {"[itemProp='priceCurrency']", "content"},
            {"[property='og:price:currency']", "content"},
            {"[property='product:price:currency']", "content"},
        };

        /// <summary>
        /// The language declaration sources in the HTML document.
        /// <see cref="https://www.w3.org/International/questions/qa-html-language-declarations"/>
        /// </summary>
        public static readonly IDictionary<string, string[]> LANG_SOURCES = new Dictionary<string, string[]>
        {
            // 'lang' attributes on the HTML tag
            {"//html", new string[] { "lang", "xml:lang" } },
            {"//meta[@http-equiv='Content-Language']", new string[] {"content"} }
        };

        /// <summary>
        /// Parts of attribute names in the document which values can contain a product price.
        /// </summary>
        public static readonly string[] PRICE_ATTRIBUTE_NAMES = new string[] { "price", "cost" };
    }
}
