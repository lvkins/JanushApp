using System;
using System.Collections.Generic;

namespace PromoSeeker.Core
{
    public static class Consts
    {
        /// <summary>
        /// The main application name.
        /// </summary>
        public const string APP_TITLE = "PromoSeeker";

        /// <summary>
        /// The fake user agent to use for the HTTP requests.
        /// </summary>
        public const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36 OPR/58.0.3135.65";

        /// <summary>
        /// The product title maximum length.
        /// </summary>
        public const int PRODUCT_TITLE_MAX_LENGTH = 96;

        /// <summary>
        /// The time interval, product will updated within.
        /// </summary>
        public static readonly TimeSpan PRODUCT_UPDATE_INTERVAL = TimeSpan.FromSeconds(5);

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
            // Meta tag is the most trusted price source.
            {"//meta[@itemprop='price']", "content"},
            {"//meta[@property='og:price:amount']", "content"},
            {"//meta[@property='product:price:amount']", "content"},
            {"//meta[@name='twitter:data1']", "content"}, // Twitter product card - price definition
        };

        /// <summary>
        /// The sources to look for the product price currency.
        /// </summary>
        public static readonly IDictionary<string, string> CURRENCY_SOURCES = new Dictionary<string, string>
        {
            {"//meta[@itemprop='priceCurrency']", "content"},
            {"//meta[@property='og:price:currency']", "content"},
            {"//meta[@property='product:price:currency']", "content"},
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
