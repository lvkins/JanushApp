using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Janush.Core.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Janush.Core
{
    /// <summary>
    /// A main product class.
    /// </summary>
    public class Product : IDisposable
    {
        #region Private Members

        /// <summary>
        /// The document of the product web page.
        /// </summary>
        private IDocument _htmlDocument;

        /// <summary>
        /// A tracking task cancellation token.
        /// </summary>
        private CancellationTokenSource _trackingCancellation;

        /// <summary>
        /// A task that is handling tracking of this product.
        /// </summary>
        private Task _trackingTask;

        #endregion

        #region Public Properties

        /// <summary>
        /// The product URL.
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// The product name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The current product price informations.
        /// </summary>
        public PriceInfo PriceInfo { get; private set; }

        /// <summary>
        /// A list of detected prices on the product website.
        /// </summary>
        public List<PriceInfo> DetectedPrices { get; private set; }

        /// <summary>
        /// The culture of the website where the product is.
        /// </summary>
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// The product site full title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Whether the product <see cref="_htmlDocument"/> has been loaded.
        /// </summary>
        public bool IsLoaded => _htmlDocument != null && !string.IsNullOrEmpty(Title);

        /// <summary>
        /// Whether the product properties are set properly and the product is ready to be tracked.
        /// </summary>
        public bool IsReady => PriceInfo != null && Culture != null;

        /// <summary>
        /// Whether the product properties like the culture of the website, price selector and name should be loaded automatically.
        /// </summary>
        public bool IsAutoDetect { get; }

        /// <summary>
        /// Whether the product tracking task is currently active.
        /// </summary>
        public bool IsTrackingRunning => _trackingTask != null && !_trackingTask.IsCompleted;

        #endregion

        #region Public Events

        /// <summary>
        /// The event that raises when the tracking has encountered a failure.
        /// </summary>
        public event Action<Exception> TrackingFailed = (exception) => { };

        /// <summary>
        /// The event that raises when the product update has started.
        /// </summary>
        public event Action Updating = () => { };

        /// <summary>
        /// The event that raises when the product update has finished, whether successfully or not.
        /// </summary>
        public event Action<ProductUpdateResult> Updated = (result) => { };

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a product that whose data should be loaded automatically from the given <paramref name="url"/>.
        /// </summary>
        /// <param name="url">The product URL.</param>
        public Product(string url)
        {
            // Set product URL
            Url = url;

            // Auto load other necessary properties
            IsAutoDetect = true;
        }

        /// <summary>
        /// Initializes a product from the previously created product settings
        /// object, containing all relevant informations about the product.
        /// </summary>
        /// <param name="settings">The product settings object.</param>
        public Product(ProductDataModel settings)
        {
            // Set properties
            Name = settings.Name;
            Url = settings.Url.ToString();
            PriceInfo = settings.Price;
            Culture = settings.Culture;
            IsAutoDetect = settings.AutoDetect;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets a price source for the product that will be used for tracking.
        /// </summary>
        /// <param name="price"></param>
        public void SetTrackingPrice(PriceInfo price)
        {
            // Set tracking price details
            PriceInfo = price;
            // Reset detected prices
            DetectedPrices = default;
        }

        /// <summary>
        /// Navigates the product <see cref="Url"/> and downloads the document.
        /// </summary>
        /// <returns></returns>
        public async Task<ProductLoadResult> OpenAsync()
        {
            // Loader result
            WebLoaderResult result = null;

            // Attempt to load HTML
            try
            {
                // Load the product website
                result = await CoreDI.WebLoader.LoadReadyAsync(Url);

                // Dispose of any current document
                _htmlDocument?.Dispose();

                // Set document
                _htmlDocument = result?.Document;
            }
            catch (Exception e)
            {
                // Let developer know
                Debugger.Break();

                // Log details
                CoreDI.Logger.Exception(e);

                // Continue execution
            }

            // If failed to load...
            if (result == null)
            {
                // Return failure
                return new ProductLoadResult
                {
                    Success = false,
                    Error = ProductLoadResultErrorType.NoResponse
                };
            }

            // If document is not loaded properly...
            if (string.IsNullOrWhiteSpace(_htmlDocument?.DocumentElement?.TextContent) ||
                !((int)_htmlDocument.StatusCode >= 200 && (int)_htmlDocument.StatusCode <= 299))
            {
                // Leave a log message
                CoreDI.Logger.Error($"Load unsuccessful > {_htmlDocument?.StatusCode}");

                // Return failure result
                return new ProductLoadResult
                {
                    Success = false,
                    Error = _htmlDocument == null
                        ? ProductLoadResultErrorType.NoResponse
                        : ProductLoadResultErrorType.InvalidResponse,
                };
            }

            // If requested was redirected...
            if (result.Redirected)
            {
                // Handle redirected request

                // Get last segments of each URI and compare them to
                // make sure if wasn't a typical redirect like example.com to www.example.com
                var lastSegment = new Uri(Url).Segments.LastOrDefault();
                var newLastSegment = new Uri(_htmlDocument.Url).Segments.LastOrDefault();

                // If last segments don't match...
                if (!lastSegment.Equals(newLastSegment, StringComparison.OrdinalIgnoreCase))
                {
                    // Leave a log message
                    CoreDI.Logger.Error($"Redirect mismatch encountered [{lastSegment}, {newLastSegment}]");

                    // Product is probably no longer available
                    // Return failure result
                    return new ProductLoadResult
                    {
                        Success = false,
                        Error = ProductLoadResultErrorType.Redirected
                    };
                }
            }

            // Set properties
            Title = _htmlDocument.Title;

            // Return successful result
            return new ProductLoadResult
            {
                Success = true
            };
        }

        /// <summary>
        /// Loads the product.
        /// </summary>
        /// <returns>A task which result will contain a <see cref="ProductLoadResult"/> object.</returns>
        public Task<ProductLoadResult> LoadAsync()
        {
            // If properties should be automatically detected...
            return IsAutoDetect
                // Load automatically
                ? LoadAutoAsync()
                // Load manually
                : LoadManuallyAsync();
        }

        /// <summary>
        /// Starts the product tracking task with the specified time <paramref name="interval"/>.
        /// </summary>
        /// <param name="interval">The time interval to track the product within.</param>
        /// <param name="randomizeInterval">If the random seconds should be added to the interval on each data pull to prevent being suspicious.</param>
        /// <exception cref="InvalidOperationException">Thrown when product is not ready to be tracked.</exception>
        public async Task TrackAsync(TimeSpan interval, bool randomizeInterval = true)
        {
            // If the product is not set...
            if (!IsReady)
            {
                // Product has to be loaded in order to start tracking
                throw new InvalidOperationException("Product is not staged for tracking");
            }

            // If a tracking task that is not completed already exists...
            if (IsTrackingRunning)
            {
                // Do nothing, shall properly stop tracking first
                return;
            }

            // Dispose of any existing cancellation token
            _trackingCancellation?.Dispose();

            // Create fresh token
            _trackingCancellation = new CancellationTokenSource();

            // Create a task
            _trackingTask = Task.Run(async () =>
            {
                // Create random object instance
                var random = new Random();

                // Do this until not interrupted by cancellation token
                while (true)
                {
                    // The delay time
                    var delay = randomizeInterval
                        ? interval.Add(TimeSpan.FromSeconds(random.Next(-5, 5)))
                        : interval;

                    // Implicitly limit the delay to 5 seconds to prevent flood under any circumstances.
                    if (delay < TimeSpan.FromSeconds(5))
                    {
                        delay = TimeSpan.FromSeconds(5);
                    }

                    // Wait for the specified amount of time
                    await Task.Delay(delay, _trackingCancellation.Token);

                    // Abort if cancellation is requested
                    _trackingCancellation.Token.ThrowIfCancellationRequested();

                    // Update
                    await UpdateAsync();
                }
            }, _trackingCancellation.Token)
            // Handle faulted task
            .ContinueWith(t =>
            {
                // Inform developer
                Debugger.Break();

                // If we have the exception...
                if (t.Exception != null)
                {
                    // Log the exception
                    CoreDI.Logger.Exception(t.Exception.GetBaseException());
                }

                // Notify that the tracking task has failed and pass the exception details
                TrackingFailed(t.Exception?.GetBaseException());
            }, TaskContinuationOptions.OnlyOnFaulted)
            .ContinueWith(t =>
            {
                // Handle task cancellation politely and stop bubbling it up
            }, TaskContinuationOptions.OnlyOnCanceled);

            // Wait for the task to finish
            await _trackingTask;
        }

        /// <summary>
        /// Stops the product tracking task.
        /// </summary>
        public async Task StopTrackingAsync()
        {
            // Request task cancellation
            _trackingCancellation?.Cancel();

            // Wait while task is completing...
            while (IsTrackingRunning)
            {
                await Task.Delay(100);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Formats the input string by removing any line breaks, multiple whitespaces, then trims the output.
        /// </summary>
        /// <param name="input">The input string to be formatted.</param>
        /// <param name="result">When this method returns, contains the formatted <see cref="string"/> value of the <paramref name="input"/>.</param>
        private void ParseNodeText(string input, out string result) => result = Regex.Replace(HttpUtility.HtmlDecode(input), @"\s+", " ").Trim();

        /// <summary>
        /// Checks whether the input can be parsed to the <see cref="decimal"/> value using the website detected culture <see cref="Culture"/>.
        /// </summary>
        /// <param name="input">The input to be parsed.</param>
        /// <returns><see langword="true"/> if the <paramref name="input"/> value can be parsed to <see cref="decimal"/> value, otherwise <see langword="false"/>.</returns>
        private bool ReadPrice(string input, out PriceReadResult output)
        {
            // If the input is empty...
            if (string.IsNullOrEmpty(input))
            {
                // Not a price
                output = default(PriceReadResult);
                return false;
            }

            // Ensure the proper input
            ParseNodeText(input, out var result);

            // If result is empty...
            if (string.IsNullOrEmpty(result))
            {
                // Not a price
                output = default(PriceReadResult);
                return false;
            }

            // Fix price format (separators) accordingly to the current culture
            output = CurrencyHelpers.ReadPriceValue(result, Culture);

            // Return with the price valid info
            return output.Valid;
        }

        /// <summary>
        /// Attempts to locate website culture in order to find which culture to use for price conversions etc. If the culture couldn't be located
        /// the default culture of <see cref="CultureInfo.CurrentCulture"/> will be set.
        /// </summary>
        /// <returns><see langword="true"/> if the culture was located in the <see cref="_htmlDocument"/> and set, otherwise <see langword="false"/>.</returns>
        private bool DetectCulture()
        {
            #region Detect Culture By Currency Meta Tag

            // Leave a log message
            Debug.WriteLine("> Attempt to detect culture by currency meta tag...");

            // Find the currency value in the pre-defined node sources
            var currencyValue = _htmlDocument.DocumentElement.FindContentFromSource(Consts.CURRENCY_SOURCES);

            // If not empty and culture exists...
            if (!string.IsNullOrWhiteSpace(currencyValue) && CurrencyHelpers.FindCultureByCurrencySymbol(currencyValue, out var culture))
            {
                // Set the culture
                Culture = culture;

                // Success
                return true;
            }

            #endregion

            #region Detect Culture By Prices Currency

            // Leave a log message
            Debug.WriteLine("> Attempt to detect culture by prices currency...");

            // Take all prices in the document, lookup the currency symbol they use and try to find appropriate culture by the symbol.
            var topPricesCulture = _htmlDocument.Descendents()
                // Get most nested text nodes
                .Where(_ => !_.HasChildNodes && !string.IsNullOrWhiteSpace(_.TextContent))
                // Get fixed text
                .Select(_ =>
                {
                    // Prepare text
                    ParseNodeText(_.TextContent, out var result);
                    return new { Text = result, Node = _ };
                })
                // Where the value is price-like.
                // Content is not empty and has at least one digit and character, but not more than 5 non-digit characters (longest currency symbol length) + 5 in reserve for separators.
                // NOTE: We cannot use char.IsLetter check here because we are searching for either a symbol ($) or ISO symbol (USD), also some symbol characters are not recognized by .IsLetter.
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
                Culture = topPricesCulture.Culture;

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
                var node = _htmlDocument.QuerySelector(source.Key);

                // If node exists...
                if (node != null)
                {
                    // Find any attribute
                    var result = source.Value
                        .Select(_ => new { Name = _, Value = node.GetAttribute(_) })
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
                            Culture = new CultureInfo(result.Value);

                            // Success
                            return true;
                        }
                    }
                }
            }

            #endregion

            // Let developer know
            Debugger.Break();

            // Failure
            return false;
        }

        /// <summary>
        /// Attempts to extract and set the displaying product name from the <see cref="_htmlDocument"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the product name was extracted and set successfully, otherwise <see langword="false"/>.</returns>
        private bool DetectName()
        {
            // We assume that the product name is always defined in the page title.
            // Otherwise we are dealing with some badly set e-commerce site, which we don't care about right now.

            // Find the title
            var titleValue = _htmlDocument.DocumentElement.FindContentFromSource(Consts.TITLE_SOURCES);

            // If we have no page title...
            if (string.IsNullOrWhiteSpace(titleValue))
            {
                return false;
            }

            // Format the page title properly
            ParseNodeText(titleValue, out var pageTitle);

            // Get nodes that can potentially contain the product title
            var contents = _htmlDocument.DocumentElement.Descendents()
                // Only nodes without children, not empty and not longer than product max. length
                .Where(_ => !_.HasChildNodes && !string.IsNullOrWhiteSpace(_.TextContent) && _.TextContent.Length <= Consts.PRODUCT_TITLE_MAX_LENGTH)
                // Select node along with parsed text
                .Select(_ =>
                {
                    // Parse
                    ParseNodeText(_.TextContent, out var text);

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

            // Leave a message
            Console.WriteLine($"> Detected current product name: {Name}");

            // Setting the title was successful if it's not empty
            return !string.IsNullOrWhiteSpace(Name);
        }

        /// <summary>
        /// Attempts to extract all available price source from the <see cref="_htmlDocument"/>.
        /// </summary>
        /// <returns><see cref="true"/> if found any price source, otherwise <see cref="false"/>.</returns>
        private bool DetectPriceSources()
        {
            // TODO: Detect certain price from the argument passed

            // Initialize price sources
            if (DetectedPrices != null)
            {
                // Clear current content
                DetectedPrices.Clear();
            }
            else
            {
                DetectedPrices = new List<PriceInfo>();
            }

            // Iterate through pre-defined price sources
            // Prices from this source are the most eligible
            foreach (var source in Consts.PRICE_SOURCES)
            {
                // Find a node in the document
                // If node exists...
                if (_htmlDocument.DocumentElement.QuerySelectorOrXPath(source.Key) is IElement node)
                {
                    // Whether the price is defined within an attribute
                    var isAttribute = !string.IsNullOrEmpty(source.Value);

                    // Define price result
                    PriceReadResult price = null;

                    // If we are dealing with the attribute source...
                    if (isAttribute)
                    {
                        // Read from the attribute
                        ReadPrice(node.GetAttribute(source.Value), out price);
                    }

                    // If price wasn't found yet...
                    if (price?.Valid != true)
                    {
                        // NOTE: We do allow to parse node content after failed attribute read.
                        // This is because some sites not follow good practices and define
                        // values in the node content

                        // Price wasn't found in the attribute
                        isAttribute = false;

                        // Read from the content
                        ReadPrice(node.TextContent, out price);
                    }

                    // If price read was successful and price is unique...
                    if (price?.Valid == true && !DetectedPrices.Any(_ => _.Value == price.Decimal))
                    {
                        // Read from value attribute and store the price source
                        DetectedPrices.Add(new PriceInfo(price.Decimal, Culture)
                        {
                            PriceXPathOrSelector = source.Key, // pre-defined source selector is better than generated
                            AttributeName = isAttribute ? source.Value : null,
                            Source = isAttribute
                                ? PriceSourceType.PriceSourceAttribute
                                : PriceSourceType.PriceSourceNode,
                        });
                    }
                }
            }

            // If we have any prices from the pre-defined sources...
            if (DetectedPrices.Any())
            {
                // We have prices from a reliable source, so we can return at this point.
                return true;
            }

            // If the above failed, attempt to locate price by parsing the document.
            // Some e-commerce sites just make it hard sometimes and decide not to follow the good practices.

            // Create compiled regex instance to find a product name in the document
            var ProductNameRegex = new Regex($@"\b{Regex.Escape(Name)}\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // Create compiled regex instance to find the Javascript objects values in the document, where key contains either a 'price' or 'cost' word.
            // Sample matches:
            //  ['myPrice' : 123.00,] -> 123.00
            //  ['productCost' : 1234,] -> 1234
            //  ["cost" : 100,] -> 100
            var PricesInJavaScriptRegex = new Regex(@"\b[\""\']?(?:[\w\-]+)?(?:price|cost)(?:[\w\-]+)?[\""\'\s]?\:[\""\'\s]?([\d\.\,\ ]+)[\""\']?\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            // The maximum allowed number of depth between price and the product name price node
            var MaxNameNodeDistance = 7;

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
            //      NOTE that Javascript prices are (currently) used only for a reference. 
            //      This is because of the fact that these prices are not tied to 
            //      any specific node in DOM.
            //
            //
            //  2. The values from attributes named after price, eg. data-my-price="1234".
            //     Some e-commerce sites declare the prices in the tag attributes. Accurate or not,
            //     this is a worthy method to get a valid price from.
            //
            //
            //  3. Parse most nested nodes inner text that display the price on the screen.
            //     Every e-commerce site has to print the price to the user.
            //     Despite some cases where the price is shown through images/drawing. This is a really good source of information.
            //

            // Extract price values in the Javascript objects declared in the document
            var pricesInJavaScript = PricesInJavaScriptRegex
                .Matches(_htmlDocument.DocumentElement.TextContent)
                .Cast<Match>()
                .Select(m => new { IsPrice = ReadPrice(m.Groups[1].Value, out var price), Price = price })
                .Where(_ => _.IsPrice && _.Price.Decimal > 0)
                .Select(_ => new
                {
                    Node = default(IElement),
                    PriceReadResult = _.Price,
                    PriceInfo = new PriceInfo(_.Price.Decimal, Culture)
                    {
                        Source = PriceSourceType.PriceSourceJavascript,
                    }
                });

            // Get all document descendants
            var docDescendants = _htmlDocument.Descendents();

            // Extract price values from attributes named after prices.
            var pricesInAttributeValues = docDescendants
                // HTML elements descendants only
                .OfType<IElement>()
                // Having at least one attribute
                .Where(_ => _.Attributes.Any())
                .SelectMany(_ => _.Attributes
                    .Where(a => a.Name.ContainsAny(Consts.PRICE_ATTRIBUTE_NAMES))
                    .Select(a => new { IsPrice = ReadPrice(a.Value, out var price), Price = price, Attribute = a })
                    .Where(a => a.IsPrice && a.Price.Decimal > 0)
                    .Select(a => new
                    {
                        Node = _,
                        PriceReadResult = a.Price,
                        PriceInfo = new PriceInfo(a.Price.Decimal, Culture)
                        {
                            Source = PriceSourceType.PriceSourceAttribute,
                            AttributeName = a.Attribute.Name,
                        }
                    })
                );

            // Extract price values from most nested nodes
            var pricesInNodes = docDescendants
                // Get text nodes only
                .OfType<IText>()
                // Where text length of least two characters and doesn't have link ancestor
                .Where(_ => _.Length > 1 && !_.Ancestors().OfType<IHtmlAnchorElement>().Any())
                // Select price
                .Select(_ => new
                {
                    IsPrice = ReadPrice(_.TextContent, out var price),
                    Node = _.ParentElement,
                    PriceReadResult = price
                })
                // Get only successfully parsed prices
                .Where(_ => _.IsPrice && _.PriceReadResult.Decimal > 0)
                // Continue with same types as in other prices for the concatenation data type compatibility
                .Select(_ => new
                {
                    _.Node,
                    _.PriceReadResult,
                    PriceInfo = new PriceInfo(_.PriceReadResult.Decimal, Culture)
                    {
                        Source = PriceSourceType.PriceSourceNode,
                    }
                });

            // Get best prices by concatenating three source lists and scoring each price group.
            DetectedPrices = pricesInJavaScript
                .Concat(pricesInAttributeValues)
                .Concat(pricesInNodes)
                // Find each node price distance to product name.
                .Select(_ =>
                {
                    // Distance from the price node stream position to the closest product name node stream position.
                    int? nameDistance = null;

                    // If price has a source node and source price is a price found in the node text (node that *could be* displayed to user)...
                    if (_.Node != null && _.PriceInfo.Source == PriceSourceType.PriceSourceNode)
                    {
                        // NOTE: Since we've switched from HtmlAgilityPack to AngleSharp
                        // We have lost the ability to check the stream position of the element.
                        // Thats why we use the node 'depth' approach to get the distance - it's not that accurate however.
                        // Stream position should be added soon: https://github.com/AngleSharp/AngleSharp/issues/754

                        // Find closest product name position
                        var result = _.Node.FindClosest(n =>
                        {
                            return ProductNameRegex.IsMatch(n.TextContent);
                            //return n.InnerText.ContainsEx(ProductName, StringComparison.Ordinal);
                        }, out var node, out var distance);

                        // If closest node was found...
                        if (result)
                        {
                            // Set a distance to the node
                            nameDistance = distance; //_.SourceNode.StreamPosition() - distanceToProductName;
                        }
                    }

                    // Pass the anonymous type
                    return new
                    {
                        _.Node,
                        _.PriceReadResult,
                        _.PriceInfo,
                        NameDistance = nameDistance,
                    };
                })
                // Group entries by the price and prioritize prices having node sources
                .GroupBy(_ => _.PriceReadResult.Decimal, (k, g) => new { Key = k, Prices = g.OrderByDescending(p => p.Node != null) })
                // Select price & entries count
                .Select(_ =>
                {
                    // Check if any price in this group occurs with the currency symbol in the document...
                    // Depending on the website and how the price is presented to the user, at least once price should have a currency symbol.
                    var hasSymbol = _.Prices.Any(p => !string.IsNullOrEmpty(p.PriceReadResult.CurrencySymbol));

                    #region Make Counts

                    // Particular source counts
                    int inAttrCount = 0, inJSCount = 0, inNodeCount = 0, totalCount = 0;

                    // Get particular prices by source count
                    foreach (var item in _.Prices)
                    {
                        if (item.PriceInfo.Source == PriceSourceType.PriceSourceAttribute)
                        {
                            ++inAttrCount;
                        }
                        else if (item.PriceInfo.Source == PriceSourceType.PriceSourceJavascript)
                        {
                            ++inJSCount;
                        }
                        else if (item.PriceInfo.Source == PriceSourceType.PriceSourceNode)
                        {
                            ++inNodeCount;
                        }

                        ++totalCount;
                    }

                    #endregion

                    #region Compute Price Group Score

                    // Base score
                    var groupScore = totalCount;

                    // We assume, the price appears in either the attributes or the JS code at least once...
                    if (inAttrCount != 0 || inJSCount != 0)
                    {
                        // If all prices don't come from attributes...
                        if (inAttrCount != totalCount)
                        {
                            // Bonus score for prices found in the attributes...
                            groupScore += inAttrCount * 2;
                        }
                        // Otherwise...
                        else
                        {
                            // Weak source penalty
                            groupScore -= (inAttrCount + 20);
                        }

                        // If all prices don't come from JS code...
                        if (inJSCount != totalCount)
                        {
                            // Bonus score for prices found in the JS code...
                            groupScore += inJSCount * 3;
                        }
                        // Otherwise...
                        else
                        {
                            // Weak source penalty
                            groupScore -= (inJSCount + 20);
                        }
                    }
                    // Otherwise if all prices come from the attributes/JS code...
                    else
                    {
                        // Weak source penalty
                        groupScore -= 5;
                    }

                    // Score closest price-to-name element distance

                    // If at least one price comes from a HTML node...
                    if (inNodeCount > 0)
                    {
                        // Get the closest distance of a price to the product name in this group
                        var closestNameDistance = _.Prices.Where(p => p.NameDistance > -1).Min(p => p.NameDistance);

                        // If no name element was located near the price...
                        if (closestNameDistance == null)
                        {
                            // Set for penalty
                            closestNameDistance = MaxNameNodeDistance + 1;
                        }

                        // If the distance is relatively close...
                        if (closestNameDistance > 0 && closestNameDistance <= MaxNameNodeDistance)
                        {
                            // Add bonus score
                            groupScore += 10 - (closestNameDistance.Value - 1);
                        }
                        // Otherwise, if the distance is long...
                        else if (closestNameDistance > MaxNameNodeDistance)
                        {
                            // Long distance penalty
                            // 10 points for each exceeded limit
                            groupScore -= closestNameDistance.Value / MaxNameNodeDistance * 10;
                        }
                    }
                    else
                    {
                        // Weak source penalty
                        groupScore -= 5;
                    }

                    // If any price in this group occurs with a currency symbol...
                    if (hasSymbol)
                    {
                        // Add bonus points
                        groupScore += 15;
                    }

                    #endregion

                    Debug.WriteLine($">> Price detected :: {_.Key} ({groupScore}) ({totalCount})");

                    // Return the anonymous type containing useful data
                    return new
                    {
                        Score = groupScore,
                        Source = _.Prices.Select(s => new { s.Node, s.PriceInfo }),
                    };
                })
                // Order by the group of prices score
                .OrderByDescending(_ => _.Score)
                // Take 10 best price groups
                .Take(10)
                // Since currently tracking prices without source in node
                // (eg. the Javascript prices, that are used for reference) is 
                // currently not supported. Exclude groups having no node prices at all.
                // (Comparing first element is enough, because of the previously
                // sorted node-first elements)
                .Where(_ => _.Source.First()?.Node != null)
                // Reduct to the first price in each group and create a selector for each price
                .Select(_ =>
                {
                    // Get first (top) price in a group
                    // (NOTE: Prices in the group are previously sorted node-first)
                    var firstPrice = _.Source.First();

                    // If price has a node origin...
                    if (firstPrice.Node != null)
                    {
                        // Create a CSS selector for each price
                        // NOTE: We do it here at the end, once we gather eligible 
                        //  prices - because it is pointless to waste resources to 
                        //  create selectors for inappropriate nodes.
                        firstPrice.PriceInfo.PriceXPathOrSelector = firstPrice.Node.GetSelector();
                    }

                    // Reduce
                    return firstPrice.PriceInfo;
                })
                // Make list
                .ToList();

            // If we have no prices...
            if (!DetectedPrices.Any())
            {
                Debug.WriteLine("> Unable to detect product price.");
                return false;
            }

            // Leave a log message
            Debug.WriteLine("> Prices detected");

            // We have some prices, return true
            return true;
        }

        /// <summary>
        /// Automatically loads the latest product properties.
        /// </summary>
        /// <returns>A task that will finish once auto loading is finished. Task result contains the result of the product load.</returns>
        private async Task<ProductLoadResult> LoadAutoAsync()
        {
            // Once we have the HTML document, we can parse product properties
            if (_htmlDocument == null)
            {
                // Open product website
                var loadResult = await OpenAsync();

                // If not successful...
                if (!loadResult.Success)
                {
                    return loadResult;
                }
            }

            // Create the result
            var result = new ProductLoadResult();

            // NOTE: Order of the loading properties is important

            // If failed to detect product website culture...
            if (!DetectCulture())
            {
                result.Error = ProductLoadResultErrorType.ProductUnknownCulture;
            }
            // If failed to detect product name...
            else if (!DetectName())
            {
                result.Error = ProductLoadResultErrorType.ProductUnknownName;
            }
            // If failed to detect price sources...
            else if (!DetectPriceSources())
            {
                result.Error = ProductLoadResultErrorType.ProductUnknownPrice;
            }
            // Success
            else
            {
                // Set successful result
                result.Success = true;

                // Set the best-scored price detected to use
                PriceInfo = DetectedPrices.First();
            }

            // Return the result
            return result;
        }

        /// <summary>
        /// Manually loads the latest product properties.
        /// </summary>
        /// <returns>A task that will finish once loading is finished. Task result contains the result of the product load.</returns>
        private async Task<ProductLoadResult> LoadManuallyAsync()
        {
            // Once we have the HTML document, we can parse product properties
            if (_htmlDocument == null)
            {
                // Open product website
                var loadResult = await OpenAsync();

                // If not successful...
                if (!loadResult.Success)
                {
                    return loadResult;
                }
            }

            // Create the result
            var result = new ProductLoadResult { Success = true };

            // Ensure we got all necessary properties set
            if (Culture == null || string.IsNullOrEmpty(Name) || PriceInfo == null)
            {
                result.Success = false;
                result.Error = ProductLoadResultErrorType.ProductParamNotFound;
                return result;
            }

            #region Set Price

            // Select node
            var node = _htmlDocument.Body.QuerySelectorOrXPath(PriceInfo.PriceXPathOrSelector);

            // If node wasn't located in the DOM...
            if (node == null)
            {
                // Return failure
                result.Success = false;
                return result;
            }

            // Set price source to node
            PriceInfo.Source = PriceSourceType.PriceSourceNode;

            // Find price anywhere in the node (including attributes)

            // Try parse node content and if result is not a valid price...
            if (!ReadPrice(node.TextContent, out var price))
            {
                // Try parse attribute values
                var attribute = node.Attributes
                    .FirstOrDefault(_ => _.Name.ContainsAny(Consts.PRICE_ATTRIBUTE_NAMES) && ReadPrice(_.Value, out price));

                // If failed to find price in the attributes...
                if (string.IsNullOrEmpty(attribute?.Value))
                {
                    // Return failure
                    result.Success = false;
                    result.Error = ProductLoadResultErrorType.ProductUnknownPrice;
                    return result;
                }

                // Set attribute, where the price was found
                PriceInfo.AttributeName = attribute.Name;
                // Set proper price source
                PriceInfo.Source = PriceSourceType.PriceSourceAttribute;
            }

            // Recreate price
            PriceInfo = new PriceInfo(price.Decimal, Culture)
            {
                PriceXPathOrSelector = PriceInfo.PriceXPathOrSelector,
                AttributeName = PriceInfo.AttributeName,
                Source = PriceInfo.Source
            };

            #endregion

            // Return the result
            return result;
        }

        /// <summary>
        /// Load current, up-to-date product properties.
        /// </summary>
        /// <returns>A task that will finish once the product is updated, the task result contains the update result object.</returns>
        private async Task<ProductUpdateResult> UpdateAsync()
        {
            // Raise updating event
            Updating();

            // Load fresh document
            var loadResult = await OpenAsync();

            // Create update result object
            var updateResult = new ProductUpdateResult
            {
                Success = true
            };

            // If load wasn't successful...
            if (!loadResult.Success)
            {
                // Set failed result
                updateResult.Success = false;
                updateResult.Error = loadResult.Error.ToDisplayString();

                // Raise updated event
                Updated(updateResult);

                // Return, since we have no result to parse
                return updateResult;
            }

            // Reload

            #region Update Name

            // NOTE: We always use automatic detection for product
            //  name, if user adds product manually and types whatever
            //  name, then such name will remain as a custom product name.

            // If properties should be detected automatically...
            if (IsAutoDetect)
            {
                // Store current name
                var preName = Name;

                // Get latest product name
                if (!DetectName())
                {
                    // Set failed result
                    updateResult.Success = false;
                    updateResult.Error = Strings.ErrorProductUpdateName;
                }
                // If names are not equal...
                else if (!preName.Equals(Name, StringComparison.InvariantCulture))
                {
                    // Flag that name differs
                    updateResult.HasNewName = true;
                }
            }

            #endregion

            #region Update Price

            // Get latest product price
            var priceNode = _htmlDocument.DocumentElement.QuerySelectorOrXPath(PriceInfo.PriceXPathOrSelector);

            // If node was selected successfully...
            if (priceNode != null)
            {
                // Read price
                var input = priceNode.TextContent;

                // If price is located within an attribute
                if (!string.IsNullOrEmpty(PriceInfo.AttributeName))
                {
                    input = priceNode.Attributes[PriceInfo.AttributeName]?.Value;
                }

                // Read price in the input
                var result = ReadPrice(input, out var parseResult);

                // If the input was parsed and price differs from the current price...
                if (result && parseResult.Decimal != PriceInfo.Value)
                {
                    // Update price details
                    PriceInfo = new PriceInfo(parseResult.Decimal, Culture)
                    {
                        AttributeName = PriceInfo.AttributeName,
                        PriceXPathOrSelector = PriceInfo.PriceXPathOrSelector,
                        Source = PriceInfo.Source
                    };

                    updateResult.HasNewPrice = true;
                }
                else if (!result)
                {
                    // Let developer know
                    Debugger.Break();

                    // Set failed result
                    updateResult.Success = false;
                    updateResult.Error = Strings.ErrorProductUpdatePriceRead;
                }
            }
            else
            {
                // Price element wasn't located in the DOM

                // TODO: If IsAutoDetect, then attempt to detect new price selector

                // Set failed result
                updateResult.Success = false;
                updateResult.Error = Strings.ErrorProductUpdatePriceElementNotFound;
            }

            #endregion

            // Raise updated event
            Updated(updateResult);

            // Return with the result
            return updateResult;
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Cleans up the resources used.
        /// </summary>
        public void Dispose()
        {
            _htmlDocument?.Dispose();
        }

        #endregion
    }
}
