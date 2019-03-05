using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace PromoSeeker
{
    public class Promo
    {
        /// <summary>
        /// Will match anything similar to price that is comma, dot, whitespace separated or is not separated at all, excluding leading zeros.
        /// 00.1234 => 0.1234
        /// </summary>
        private readonly string mPriceRegex = @"(?!0+[^\.\, ])[0-9]+([\.\, ][0-9]{3})?([\.\, ][0-9]{1,3})?";
        // \b((?![0\.\,\ ]+$)[0-9]+([\.\,\ ][0-9]{3})?([\.\,\ ][0-9]{1,3})?)\b

        private HtmlDocument mHtmlDocument;

        private CultureInfo mCulture;


        #region Public Properties

        /// <summary>
        /// The product URL.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// The product name.
        /// </summary>
        public string Product { get; set; }

        /// <summary>
        /// The product price.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; private set; }

        #endregion

        #region Constructor

        public Promo(string url)
        {
            // Store the promotion URL
            Url = url;

            // Parse and store the HTML document
            mHtmlDocument = Parser.Instance.Parse(url);

            // Find language and set the culture
            if (SetCulture())
            {
                // Leave a log message
                Console.WriteLine($"Culture detected: {mCulture.EnglishName} ({mCulture.Name})");
            }
            else
            {
                // Fallback to default culture
                //Console.WriteLine("> No culture was detected. Fallback to local culture");
                //mCulture = CultureInfo.CurrentCulture;

                // Cannot continue without valid culture.
                // TODO: Prompt the user to specify the culture/currency the website is using manually.
                throw new NullReferenceException("Culture not set");
            }

            // Find the product title
            SetTitle();

            // Find the product price
            SetPrice();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Attempts to locate website culture in order to find which culture to use for price conversions etc. If the culture couldn't be located
        /// the default culture of <see cref="CultureInfo.CurrentCulture"/> will be set.
        /// </summary>
        /// <returns><see langword="true"/> if the culture was located in the <see cref="mHtmlDocument"/> and set, otherwise <see langword="false"/>.</returns>
        private bool SetCulture()
        {
            #region Detect Culture By Currency Meta Tag

            // Find the currency value in the pre-defined node sources
            var currencyValue = mHtmlDocument.DocumentNode.FindSourceContent(Consts.CURRENCY_SOURCES);

            // If not empty and culture exists...
            if (!string.IsNullOrWhiteSpace(currencyValue) && CurrencyHelpers.FindCultureByCurrencySymbol(currencyValue, out var culture))
            {
                // Set the culture
                mCulture = culture;

                // Success
                return true;
            }

            // Log failure
            Debug.WriteLine("> [FAIL] Detect Culture By Currency Meta Tag");

            #endregion

            #region Detect Culture By Prices Currency

            // Take all prices in the document, lookup the currency symbol they use and try to find appropriate culture by the symbol.
            var topPricesCulture = mHtmlDocument.DocumentNode.Descendants()
                // Get most nested text nodes
                .Where(_ => !_.HasChildNodes)
                // Get fixed text
                .Select(_ =>
                {
                    // Prepare text
                    ParseText(_.InnerText, out var result);
                    return new { Text = result, Node = _ };
                })
                // Where the value is price-like.
                // Content is not empty and has at least one digit and character, but not more than 5 non-digit characters (longest currency symbol length) + 5 in reserve for separators.
                // NOTE: We cannot use char.IsLetter check here because we are using for either a symbol ($) or ISO symbol (USD), also some symbol characters are not recognized by .IsLetter.
                .Where(_ => !string.IsNullOrEmpty(_.Text) && _.Text.Any(char.IsDigit) && _.Text.Count(c => !char.IsWhiteSpace(c) && !char.IsDigit(c)).InRange(1, 10))
                // Get the culture
                .Select(_ =>
                {
                    // Find culture and price
                    CurrencyHelpers.FindCultureByPrice(_.Text, out var result);

                    return new
                    {
                        SourceText = _.Text,
                        Culture = result,
                    };
                })
                // Where culture was found
                .Where(_ => _.Culture != null)
                // Group by culture identifier
                .GroupBy(_ => _.Culture.LCID)
                // Get counts
                .Select(_ => new { _.First().Culture, Count = _.Count() })
                // Order by descending count value
                .OrderByDescending(_ => _.Count)
                // Get top result
                .FirstOrDefault();

            // If culture was determined...
            if (topPricesCulture != null)
            {
                // Set the culture
                mCulture = topPricesCulture.Culture;

                // Success
                return true;
            }

            // Log failure
            Debug.WriteLine("> [FAIL] Detect Culture By Prices Currency");

            #endregion

            #region Detect Culture By Language Definition

            // Last chance - not accurate - because website language doesn't necessarily mean currency.
            // Attempt to locate website culture in order to find which culture to use for price conversions etc.
            foreach (var source in Consts.LANG_SOURCES)
            {
                // Select node by XPath expression
                var node = mHtmlDocument.DocumentNode.SelectSingleNode(source.Key);

                // If node exists...
                if (node != null)
                {
                    // Find any attribute
                    var result = source.Value
                        .Select(_ => new { Name = _, Value = node.GetAttributeValue(_, default(string)) })
                        .FirstOrDefault(_ => !string.IsNullOrWhiteSpace(_.Value));

                    // If an attribute was found...
                    if (result != null)
                    {
                        Console.WriteLine($"> Found language attribute ({result.Name}) -> [{result.Value}]");

                        // Check if such culture name can be found
                        var exists = CultureInfo.GetCultures(CultureTypes.AllCultures)
                            .Any(_ => string.Equals(_.Name, result.Value, StringComparison.OrdinalIgnoreCase));

                        // If exists...
                        if (exists)
                        {
                            // Set website culture
                            mCulture = new CultureInfo(result.Value);

                            // Success
                            return true;
                        }
                    }
                }
            }

            #endregion


            // Failure
            return false;
        }

        /// <summary>
        /// Attempts to extract and set the displaying product title from the <see cref="mHtmlDocument"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the title was extracted and set successfully, otherwise <see langword="false"/>.</returns>
        private bool SetTitle()
        {
            // We assume that the product name is always defined in the page title.
            // Otherwise we are dealing with some badly set e-commerce site, which we don't care about right now.

            // Find the title
            var titleValue = mHtmlDocument.DocumentNode.FindSourceContent(Consts.TITLE_SOURCES);

            // If we have no page title...
            if (string.IsNullOrWhiteSpace(titleValue))
            {
                return false;
            }

            // Format the page title properly
            ParseText(titleValue, out var pageTitle);

            // Get nodes that can potentially contain the product title
            var contents = mHtmlDocument.DocumentNode.Descendants()
                // Only nodes without children, not empty and not longer than product max. length
                .Where(_ => !_.HasChildNodes && !string.IsNullOrWhiteSpace(_.InnerText) && _.InnerText.Length <= Consts.PRODUCT_TITLE_MAX_LENGTH)
                // Select node along with parsed text
                .Select(_ =>
                {
                    // Parse
                    ParseText(_.InnerText, out var text);

                    // Some shops can format the page title like so:
                    // The Product Name, SiteName.com
                    // Since the product name is 'The Product Name' (without trailing comma)
                    // We need to get rid of it, before comparing.
                    text = text.TrimEnd(',', '.');

                    // Create anonymous type
                    return new { Node = _, Text = text };
                })
                // Ignore empty records after parsing
                .Where(_ => !string.IsNullOrEmpty(_.Text))
                .ToList();

            // Title should occur in the page content at least once.
            // Find title with most occurrences in the document.

            // Split the page title
            var pieces = pageTitle.Split(' ');

            // If we have title pieces...
            if (pieces.Any())
            {
                // Title occurrences counter
                var prevCount = 0;

                // Reduce page title by one word until we find the valid product title
                for (var i = pieces.Length - 1; i != 0; i--)
                {
                    // Get possible title
                    var result = string.Join(" ", pieces, 0, i).TrimEnd(',', '.');

                    // Get occurrences count in the content
                    var count = contents.Count(_ => _.Text.Equals(result, StringComparison.OrdinalIgnoreCase));

                    // If title occurs higher amount of times...
                    if (count > prevCount)
                    {
                        // Set most accurate title
                        pageTitle = result;
                        prevCount = count;
                    }
                }
            }

            // Set title
            Title = pageTitle;

            // Leave a message
            Console.WriteLine($"> Setting product title: {Title}");

            // Setting the title was successful if it's not empty
            return !string.IsNullOrWhiteSpace(Title);
        }

        /// <summary>
        /// Formats the input string by removing any line breaks, multiple whitespaces, then trims the output.
        /// </summary>
        /// <param name="input">The input string to be formatted.</param>
        /// <param name="result">When this method returns, contains the formatted <see cref="string"/> value of the <paramref name="input"/>.</param>
        private void ParseText(string input, out string result) => result = Regex.Replace(HttpUtility.HtmlDecode(input), @"\s+", " ").Trim();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IList<IPriceSource> FindPriceSources()
        {
            // Found price sources
            var ret = new List<IPriceSource>();

            // Iterate through pre-defined price sources
            foreach (var source in Consts.PRICE_SOURCES)
            {
                // Find a node in the document
                var node = mHtmlDocument.DocumentNode.SelectSingleNode(source.Key);

                // If node exists...
                if (node != null && CurrencyHelpers.ExtractPrice(node.GetAttributeValue(source.Value, default(string)), out var price, mCulture))
                {
                    // Read from value attribute and store the price source
                    ret.Add(new PriceSource
                    {
                        Price = price,
                        XPath = source.Key,
                    });
                }
            }

            // If the above failed, attempt to locate price by parsing the document.
            // Some e-commerce sites just make it hard sometimes and decide not to use the good practices.

            // Get all document descendants
            var docDescendants = mHtmlDocument.DocumentNode.Descendants();

            // Get values from attributes named price-like
            var attrValues = docDescendants
                .Where(_ => _.HasAttributes)
                .SelectMany(_ => _.Attributes
                    .Where(a => a.Name.ContainsAny(Consts.PRICE_ATTRIBUTE_NAMES) && IsPriceFormat(a.DeEntitizeValue))
                    .Select(a =>
                    {
                        CurrencyHelpers.ExtractPrice(a.Value, out var price, mCulture);

                        return new PriceSource
                        {
                            Price = price,
                            AttributeName = a.Name,
                            XPath = _.XPath
                        };
                    })
                )
                .ToList();

            var commonPriceLike = docDescendants
                .Where(_ => !_.HasChildNodes && !string.IsNullOrWhiteSpace(_.InnerText) && IsPriceFormat(_.InnerText))
                .Select(_ =>
                {
                    CurrencyHelpers.ExtractPrice(_.InnerText, out var price, mCulture);

                    if (_.InnerText.Contains(mCulture.NumberFormat.CurrencySymbol))
                    {
                        ;
                    }

                    return new PriceSource
                    {
                        Price = price,
                        SourceText = _.InnerText,
                        XPath = _.XPath,
                    };
                })
                .Concat(attrValues)
                .Where(_ => _.Price > 0)
                // Group entries by the price
                .GroupBy(_ => _.Price)
                // Select price & entries count
                .Select(_ => new { Price = _.Key, Source = _.ToList(), AllValIsAttr = _.All(s => !string.IsNullOrEmpty(s.AttributeName)), Count = _.Count() })
                .Where(_ => !_.Source.All(s => !string.IsNullOrEmpty(s.AttributeName)))
                // Order by group count
                .OrderByDescending(_ => _.Count)
                .ToList();


            ;

            // Guess price
            // TODO: Can make it point based, eg. if string contains currency symbol (zł, PLN, €, $) then such string is placed higher

            var attrValues1 = docDescendants
                .Where(_ => _.HasAttributes)
                .SelectMany(_ => _.Attributes
                    .Where(a => a.Name.ContainsAny(Consts.PRICE_ATTRIBUTE_NAMES) && IsPriceFormat(a.DeEntitizeValue))
                    .Select(a => a.Value)
                )
                .ToList();

            var commonPriceLike1 = docDescendants
                .Where(_ =>
                {
                    return !_.HasChildNodes && !string.IsNullOrWhiteSpace(_.InnerText) && IsPriceFormat(_.InnerText);
                })
                // Boil down to node content
                .Select(_ => _.InnerText)
                // Join attribute values that might contain price
                .Concat(attrValues1)
                // Extract price
                .Select(_ =>
                {
                    // Get decimal price formatted accordingly to the culture
                    CurrencyHelpers.ExtractPrice(_, out var price, mCulture);

                    // Return the anonymous type
                    return new { Price = price, Text = _ };
                })
                // Ensure valid price
                .Where(_ =>
                {
                    return _.Price > 0;
                })
                // Group entries by the price
                .GroupBy(_ => _.Price)
                // Select price & entries count
                .Select(_ => new { Price = _.Key, Source = _.ToList(), Count = _.Count() })
                // Order by group count
                .OrderByDescending(_ => _.Count)
                .ToList();

            ;

            return ret;
        }

        private bool SetPrice()
        {
            var priceSources = FindPriceSources();

            return true;
        }

        /// <summary>
        /// Checks whether the input can be parsed to the <see cref="decimal"/> value using any culture <see cref="mCulture"/> or <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        /// <param name="input">The input to be parsed.</param>
        /// <returns><see langword="true"/> if the <paramref name="input"/> value can be parsed to <see cref="decimal"/> value, otherwise <see langword="false"/>.</returns>
        private bool IsPriceFormat(string input)
        {
            // Ensure the proper input
            ParseText(input, out var result);

            // If result is empty...
            if (string.IsNullOrEmpty(result))
            {
                // Not a price
                return false;
            }

            // Price is valid if can be parsed as decimal in generic culture or the current site culture
            return decimal.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out var _) ||
                   decimal.TryParse(result, NumberStyles.Any, mCulture, out var _);
        }

        #endregion
    }
}
