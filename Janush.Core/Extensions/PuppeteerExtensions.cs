using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace Janush.Core
{
    /// <summary>
    /// The extension methods for the <see cref="PuppeteerSharp"/> package.
    /// </summary>
    public static class PuppeteerExtensions
    {
        /// <summary>
        /// Finds a certain node by a selector or XPath expression and returns it's text or the attribute(s) values, if specified.
        /// </summary>
        /// <param name="node">The node to search.</param>
        /// <param name="sources">A dictionary of XPath expressions along with the attribute names for value.</param>
        /// <param name="def">The default value.</param>
        /// <returns>The node content or attribute value if found, otherwise <paramref name="def"/> will be returned.</returns>
        public static async Task<string> FindContentFromSourceAsync<T>(this Page page, IDictionary<string, T> sources, string def = null)
        {
            // Iterate over the selectors
            foreach (var source in sources)
            {
                // Select node by a selector
                var result = await page.QuerySelectorOrXPathAsync(source.Key);

                // If node exists...
                if (result != null)
                {
                    // The value to be returned
                    var value = string.Empty;

                    // If we have single attribute to check...
                    if (source.Value is string srcVal)
                    {
                        // Set the attribute value
                        value = await result.GetAttributeAsync(srcVal);
                    }
                    else if (source.Value is string[] srcArrVal)
                    {
                        // We have array of attributes.
                        // Find any existing attribute
                        foreach (var name in srcArrVal)
                        {
                            // Find first attribute
                            var attr = await result.GetAttributeAsync(name);
                            if (!string.IsNullOrWhiteSpace(attr))
                            {
                                value = attr;
                                break;
                            }
                        }
                    }
                    else
                    {
                        // Simply get the element text
                        value = await result.GetInnerTextAsync();
                    }

                    // If we have the value...
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        // We are ready to return
                        return value;
                    }
                }
            }

            // Return default value
            return def;
        }

        /// <summary>
        /// Returns the first element within the document that matches the
        /// specified XPath expression or CSS selector.
        /// </summary>
        /// <param name="node">A node to search within.</param>
        /// <param name="selectorOrExpression">The XPath expression or CSS selector.</param>
        /// <returns>The found element.</returns>
        public static async Task<ElementHandle> QuerySelectorOrXPathAsync(this Page page, string selectorOrExpression)
        {
            // If path is a XPath query...
            if (selectorOrExpression.StartsWith("/"))
            {
                // Return result of the XPath evaluation
                return Enumerable.FirstOrDefault(await page.XPathAsync(selectorOrExpression));
            }

            // Otherwise assume CSS selector
            return await page.QuerySelectorAsync(selectorOrExpression);
        }

        public static async Task<ElementHandle> GetDocumentHandleAsync(this Page page)
        {
            return (await page.EvaluateExpressionHandleAsync("document.documentElement")) as ElementHandle;
        }

        public static Task<string> GetDocumentLanguageAsync(this Page page)
        {
            return page.EvaluateExpressionAsync<string>("document.documentElement.lang");
        }

        /// <summary>
        /// Lookup for element inner text.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns>Element inner text</returns>
        public static Task<string> GetInnerTextAsync(this ElementHandle handle)
        {
            return handle.EvaluateFunctionAsync<string>("e => e.innerText");
        }

        /// <summary>
        /// Lookup for element single attribute.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns>The element attribute value</returns>
        public static Task<string> GetAttributeAsync(this ElementHandle handle, string name)
        {
            return handle.EvaluateFunctionAsync<string>("(e, name) => e.getAttribute(name)", name);
        }

        /// <summary>
        /// Lookup for element attributes.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns>The element attributes array</returns>
        public static Task<ElementAttributes[]> GetAttributesAsync(this ElementHandle handle)
        {
            return handle.EvaluateFunctionAsync<ElementAttributes[]>("e => Array.prototype.map.call(e.attributes, _=> ({Name: _.nodeName, Value: _.nodeValue}))");
        }
    }
}
