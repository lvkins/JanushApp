using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace PromoSeeker.Core
{
    public enum ProductLoadResultType
    {
        /// <summary>
        /// Product is found valid.
        /// </summary>
        Ok,

        /// <summary>
        /// Lack of the response occurred.
        /// </summary>
        NoResponse,

        /// <summary>
        /// Invalid response occurred.
        /// </summary>
        InvalidResponse,

        /// <summary>
        /// Detected no product at all.
        /// </summary>
        ProductNotDetected,

        ProductUnknownName,
        ProductUnknownPrice,
        ProductUnknownCulture,

        /// <summary>
        /// Product was detected, but we wasn't able to acquire some of it's parameters.
        /// </summary>
        ProductParamNotFound,
        ProductNeedValidPrice,
    }

    /// <summary>
    /// The product load result object.
    /// </summary>
    public class ProductLoadResult
    {
        public bool Success { get; set; }

        public ProductLoadResultType Error { get; set; }

        public IList<IPriceSource> PricesDetected { get; set; }

        public List<string> NamesDetected { get; set; }
    }

    /// <summary>
    /// A main product class.
    /// </summary>
    public class Product
    {
        #region Private Members

        /// <summary>
        /// The HTML document of the current promotion.
        /// </summary>
        private HtmlDocument mHtmlDocument;

        /// <summary>
        /// Detected culture for the current promotion website.
        /// </summary>
        private CultureInfo mCulture;

        #endregion

        #region Public Properties

        /// <summary>
        /// The product URL.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// The product name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current product price.
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// The product price history.
        /// </summary>
        public List<double> PriceHistory { get; set; }

        /// <summary>
        /// The product site full title.
        /// </summary>
        public string Title { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="url">The product URL.</param>
        public Product(string url)
        {
            // Store the promotion URL
            Url = url;

            /*
            // Create HTML request
            var request = new HtmlRequest();

            // Load website URL
            _ = request.LoadAsync(url);

            #region Ensure Valid Response

            // If status code is any other than 200...
            if (request.Response?.StatusCode != System.Net.HttpStatusCode.OK)
            {
                // Cannot continue
                throw new Exception("Status code not OK");
            }

            #endregion

            // Store the HTML document
            mHtmlDocument = request.Document;

            // Acquire Promotion Data
            if (AutoSetup)
            {
                Setup();
            }
            */
        }

        /// <summary>
        /// Manual product constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="priceXPath"></param>
        /// <param name="culture"></param>
        public Product(string name, string url, string priceXPath, CultureInfo culture)
        {
            //
        }

        #endregion

        #region Public Methods

        public async Task<ProductLoadResult> LoadAsync()
        {
            var result = new ProductLoadResult { Success = true };

            // Create HTML request
            var request = new HtmlRequest();

            // Attempt to load HTML
            await request.LoadAsync(Url);

            // If status code is any other than 200...
            if (request.Response?.StatusCode != System.Net.HttpStatusCode.OK)
            {
                // Load wasn't successful
                result.Success = false;

                // If we have no response at all...
                if (request.Response == null)
                {
                    // No response result
                    result.Error = ProductLoadResultType.NoResponse;
                }

                // Invalid status code
                result.Error = ProductLoadResultType.InvalidResponse;

                // Return with the result
                return result;
            }

            // Store the HTML document
            mHtmlDocument = request.Document;

            // Once we have the HTML document, we can parse product properties

            // If wasn't able to detect any product info...
            // NOTE: Order is important

            if (!DetectCulture())
            {
                result.Success = false;
                result.Error = ProductLoadResultType.ProductUnknownCulture;
            }
            else if (!DetectName())
            {
                result.Success = false;
                result.Error = ProductLoadResultType.ProductUnknownName;
            }
            else
            {
                // Detect prices
                var prices = DetectPriceSources();

                if (!prices.Any())
                {
                    result.Success = false;
                    result.Error = ProductLoadResultType.ProductUnknownPrice;
                }
                // Else if we have more than one price detected...
                // TODO: Check score
                else if (prices.Count > 1)
                {
                    result.Success = false;

                    // Threat is as an error - user should be prompted to pick a valid one.
                    result.Error = ProductLoadResultType.ProductNeedValidPrice;

                    // Add the prices detected to the result
                    result.PricesDetected = prices;
                }
            }

            // Return the result
            return result;
        }


        /// <summary>
        /// Sets up the promotion product data.
        /// </summary>
        public void Setup()
        {
            // Find language and set the culture
            if (DetectCulture())
            {
                // Leave a log message
                Console.WriteLine($">> Culture detected: {mCulture.EnglishName} ({mCulture.Name})");
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

            // Find the product name
            DetectName();

            // Find the product price
            DetectPriceSources();
        }

        /// <summary>
        /// Loads a previously stored promotion data.
        /// </summary>
        public void Load()
        {

        }

        /// <summary>
        /// Store the promotion data for the next use.
        /// </summary>
        public void Save()
        {

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Attempts to locate website culture in order to find which culture to use for price conversions etc. If the culture couldn't be located
        /// the default culture of <see cref="CultureInfo.CurrentCulture"/> will be set.
        /// </summary>
        /// <returns><see langword="true"/> if the culture was located in the <see cref="mHtmlDocument"/> and set, otherwise <see langword="false"/>.</returns>
        private bool DetectCulture()
        {
            #region Detect Culture By Currency Meta Tag

            // Leave a log message
            Debug.WriteLine("> Attempt to detect culture by currency meta tag...");

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

            #endregion

            #region Detect Culture By Prices Currency

            // Leave a log message
            Debug.WriteLine("> Attempt to detect culture by prices currency...");

            // Take all prices in the document, lookup the currency symbol they use and try to find appropriate culture by the symbol.
            var topPricesCulture = mHtmlDocument.DocumentNode.Descendants()
                // Get most nested text nodes
                .Where(_ => !_.HasChildNodes)
                // Get fixed text
                .Select(_ =>
                {
                    // Prepare text
                    ParseNodeText(_.InnerText, out var result);
                    return new { Text = result, Node = _ };
                })
                // Where the value is price-like.
                // Content is not empty and has at least one digit and character, but not more than 5 non-digit characters (longest currency symbol length) + 5 in reserve for separators.
                // NOTE: We cannot use char.IsLetter check here because we are using for either a symbol ($) or ISO symbol (USD), also some symbol characters are not recognized by .IsLetter.
                .Where(_ => !string.IsNullOrEmpty(_.Text) && _.Text.Any(char.IsDigit) && _.Text.Count(c => !char.IsWhiteSpace(c) && !char.IsDigit(c)).InRange(1, 10))
                // Get the culture
                .Select(_ =>
                {
                    // Get culture from the price currency symbol
                    CurrencyHelpers.FindCurrencySymbol(_.Text, out var _, out var result);

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

            #endregion

            #region Detect Culture By Language Definition

            // Leave a log message
            Debug.WriteLine("> Attempt to detect culture by language definition...");

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
                        //Debug.WriteLine($"> Found language attribute ({result.Name}) -> [{result.Value}]");

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
        /// Attempts to extract and set the displaying product name from the <see cref="mHtmlDocument"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the product name was extracted and set successfully, otherwise <see langword="false"/>.</returns>
        private bool DetectName()
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
            ParseNodeText(titleValue, out var pageTitle);

            // Get nodes that can potentially contain the product title
            var contents = mHtmlDocument.DocumentNode.Descendants()
                // Only nodes without children, not empty and not longer than product max. length
                .Where(_ => !_.HasChildNodes && !string.IsNullOrWhiteSpace(_.InnerText) && _.InnerText.Length <= Consts.PRODUCT_TITLE_MAX_LENGTH)
                // Select node along with parsed text
                .Select(_ =>
                {
                    // Parse
                    ParseNodeText(_.InnerText, out var text);

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
                for (var i = pieces.Length; i != 0; i--)
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

            // Set product name
            Name = pageTitle;

            // Set product site title
            Title = titleValue;

            // Leave a message
            Console.WriteLine($"> Setting product name: {Name}");

            // Setting the title was successful if it's not empty
            return !string.IsNullOrWhiteSpace(Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IList<IPriceSource> DetectPriceSources()
        {
            // Found price sources
            var ret = new List<IPriceSource>();

            // Iterate through pre-defined price sources
            foreach (var source in Consts.PRICE_SOURCES)
            {
                // Find a node in the document
                var node = mHtmlDocument.DocumentNode.SelectSingleNode(source.Key);

                // If node exists...
                if (node != null && ReadPrice(node.GetAttributeValue(source.Value, default(string)), out var price))
                {
                    // Read from value attribute and store the price source
                    ret.Add(new PriceSource
                    {
                        Price = price,
                        SourceNode = node,
                    });
                }
            }

            // If the above failed, attempt to locate price by parsing the document.
            // Some e-commerce sites just make it hard sometimes and decide not to use the good practices.

            // Compiled regex to find a product name in the document.
            var ProductNameRegex = new Regex($@"\b{Regex.Escape(Name)}\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // Compiled regex to find the Javascript objects values in the document, where key contains either a 'price' or 'cost' word.
            // Sample matches:
            //  ['myPrice' : 123.00,] -> 123.00
            //  ['productCost' : 1234,] -> 1234
            //  ["cost" : 100,] -> 100
            var PricesInJavaScriptRegex = new Regex(@"\b[\""\']?(?:[\w\-]+)?(?:price|cost)(?:[\w\-]+)?[\""\'\s]?\:[\""\'\s]?([\d\.\,\ ]+)[\""\']?\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // We extract the prices in three ways:
            //
            //  1. The Javascript objects values, if the key contains 'price' keyword.
            //     Some e-commerce sites declare the prices in the Javascript objects.
            //     
            //     Will match those:
            //
            //      *word boundary*
            //      'mightBeAPriceIHope' : 1234,
            //      "price" : 4321,
            //      *word boundary*
            //
            //  2. The values from attributes named after price, eg. data-my-price="1234".
            //     Some e-commerce sites declare the prices in the tag attributes. Accurate or not,
            //     this is a worthy method to validate the price.
            //
            //
            //  3. Parse most nested nodes inner text that display the price on the screen.
            //     Every e-commerce site has to print the price to the user.
            //     Despite some cases where the price is shown through images/drawing. This is a really good source of information.
            //

            // Extract price values in the Javascript objects declared in the document
            var pricesInJavaScript = PricesInJavaScriptRegex
                .Matches(mHtmlDocument.DocumentNode.InnerText)
                .Cast<Match>()
                .Select(m => new { IsPrice = ReadPrice(m.Groups[1].Value, out var price), Price = price })
                .Where(_ => _.IsPrice && _.Price.Decimal > 0)
                .Select(_ => new PriceSource
                {
                    Price = _.Price,
                    Source = PriceSourceType.PriceSourceJavascript,
                });

            // Get all document descendants
            var docDescendants = mHtmlDocument.DocumentNode.Descendants();

            // Extract price values from attributes named after prices.
            var pricesInAttributeValues = docDescendants
                .Where(_ => _.HasAttributes)
                .SelectMany(_ => _.Attributes
                    .Where(a => a.Name.ContainsAny(Consts.PRICE_ATTRIBUTE_NAMES))
                    .Select(a => new { IsPrice = ReadPrice(a.DeEntitizeValue, out var price), Price = price, Attribute = a })
                    .Where(a => a.IsPrice && a.Price.Decimal > 0)
                    .Select(a =>
                    {
                        return new PriceSource
                        {
                            Price = a.Price,
                            Source = PriceSourceType.PriceSourceAttribute,
                            AttributeName = a.Attribute.Name,
                            SourceNode = _,
                        };
                    })
                )
                .ToList();

            // Extract price values from most nested nodes
            var pricesInNodes = docDescendants
                .Where(_ => !_.HasChildNodes && !string.IsNullOrWhiteSpace(_.InnerText))
                .Select(_ => new
                {
                    IsPrice = ReadPrice(_.InnerText, out var price),
                    PriceSource = new PriceSource
                    {
                        Price = price,
                        Source = PriceSourceType.PriceSourceText,
                        SourceNode = _,
                    }
                })
                .Where(_ => _.IsPrice && _.PriceSource.Price.Decimal > 0)
                .Select(_ => _.PriceSource);

            // Get best prices by concatenating three source lists and scoring each price group.
            var bestPrices = pricesInJavaScript
                .Concat(pricesInAttributeValues)
                .Concat(pricesInNodes)
                // Attempt to find a currency symbol in each price
                .Select(_ =>
                {
                    // Distance from the price node stream position to the closest product name node stream position.
                    int? nameDistance = null;

                    // If price has a source node and source price is a price found in the node text (node that *could be* displayed to user)...
                    if (_.SourceNode != null && _.Source == PriceSourceType.PriceSourceText)
                    {
                        // Find closest product name position
                        var distanceToProductName = _.SourceNode.FindClosest(n =>
                {
                    return ProductNameRegex.IsMatch(n.InnerText);
                    //return n.InnerText.ContainsEx(ProductName, StringComparison.Ordinal);
                }, 30)?.StreamPosition;

                        // Set the distance to the node
                        nameDistance = _.SourceNode.StreamPosition - distanceToProductName;
                    }

                    // Pass the anonymous type
                    return new
                    {
                        PriceSource = _,
                        NameDistance = nameDistance,
                    };
                })
                // Group entries by the price
                .GroupBy(_ => _.PriceSource.Price.Decimal)
                // Select price & entries count
                .Select(_ =>
                {
                    // Count of the same prices in this group
                    var count = _.Count();

                    // Particular source counts
                    int inAttrCount = 0, inJSCount = 0, inNodeCount = 0;

                    // Get particular prices by source count
                    foreach (var item in _)
                    {
                        if (item.PriceSource.Source == PriceSourceType.PriceSourceAttribute)
                        {
                            ++inAttrCount;
                        }
                        else if (item.PriceSource.Source == PriceSourceType.PriceSourceJavascript)
                        {
                            ++inJSCount;
                        }
                        else if (item.PriceSource.Source == PriceSourceType.PriceSourceText)
                        {
                            ++inNodeCount;
                        }
                    }

                    // Get the closest distance of a price to the product name in this group
                    var closestNameDistance = _.Min(p => p.NameDistance);

                    // Check if any price in this group occurs with the currency symbol in the document...
                    // Depending on the website and how the price is presented to the user, at least once price should have a currency symbol.
                    var hasSymbol = _.Any(p => !string.IsNullOrEmpty(p.PriceSource.Price.CurrencySymbol));

                    #region Compute Price Group Score

                    // Base score
                    var groupScore = count;

                    // Bonus score for prices found in the JS code...
                    groupScore += inJSCount * 3;

                    // Bonus score for prices found in the attributes...
                    groupScore += inAttrCount * 2;

                    // If all prices comes from attributes or JS...
                    if (inAttrCount == count)
                    {
                        // Weak source penalty
                        groupScore -= (inAttrCount + 20);
                    }
                    else if (inJSCount == count)
                    {
                        // Weak source penalty
                        groupScore -= (inJSCount + 20);
                    }
                    else if (inAttrCount == 0 && inJSCount == 0)
                    {
                        // Weak source penalty
                        groupScore -= 5;
                    }

                    // If there is a price with closest name...
                    if (closestNameDistance != null)
                    {
                        // If the distance is relatively close...
                        if (closestNameDistance > 0 && closestNameDistance <= 10000)
                        {
                            // Add bonus score
                            groupScore += (10 - ((int)closestNameDistance / 1000));
                        }
                        // Otherwise, if the distance is long...
                        else if (closestNameDistance > 50000)
                        {
                            // Long distance penalty
                            groupScore -= (int)closestNameDistance / 10000;
                        }
                    }

                    // If any price in this group occurs with a currency symbol in the document...
                    if (hasSymbol)
                    {
                        // Add bonus points
                        groupScore += 15;
                    }

                    #endregion

                    // Return the anonymous type containing useful data
                    return new
                    {
                        Price = _.Key,
                        Count = count,
                        InAttrCount = inAttrCount,
                        InJSCount = inJSCount,
                        Score = groupScore,
                        MinNameDistance = closestNameDistance,
                        HasSymbol = hasSymbol,
                        Source = _.Select(s => s.PriceSource),
                    };
                })
                // Order by the group of prices score
                .OrderByDescending(_ => _.Score)
                // Take 10 best prices
                .Take(10)
                // Make list
                .ToList();

            if (!bestPrices.Any())
            {
                Console.WriteLine("> Couldn't detect product price.");
                return null;
            }

            Console.WriteLine("> Prices detected");

            foreach (var item in bestPrices)
            {
                Console.WriteLine($" >> {item.Price} ({item.Score})");
                ret.AddRange(item.Source);
            }

            return ret;
        }

        /// <summary>
        /// Formats the input string by removing any line breaks, multiple whitespaces, then trims the output.
        /// </summary>
        /// <param name="input">The input string to be formatted.</param>
        /// <param name="result">When this method returns, contains the formatted <see cref="string"/> value of the <paramref name="input"/>.</param>
        private void ParseNodeText(string input, out string result) => result = Regex.Replace(HttpUtility.HtmlDecode(input), @"\s+", " ").Trim();

        /// <summary>
        /// Checks whether the input can be parsed to the <see cref="decimal"/> value using the website detected culture <see cref="mCulture"/>.
        /// </summary>
        /// <param name="input">The input to be parsed.</param>
        /// <returns><see langword="true"/> if the <paramref name="input"/> value can be parsed to <see cref="decimal"/> value, otherwise <see langword="false"/>.</returns>
        private bool ReadPrice(string input, out PriceValue output)
        {
            // Ensure the proper input
            ParseNodeText(input, out var result);

            // If result is empty...
            if (string.IsNullOrEmpty(result))
            {
                // Not a price
                output = default(PriceValue);
                return false;
            }

            // Fix price format (separators) accordingly to the current culture
            output = CurrencyHelpers.ReadPriceValue(input, mCulture);

            // Return with the price valid info
            return output.Valid;
        }

        #endregion
    }
}
