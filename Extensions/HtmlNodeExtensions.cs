using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace PromoSeeker
{
    public static class HtmlNodeExtensions
    {
        /// <summary>
        /// Finds a certain node by XPath selector and returns it's text or the attribute value, if specified.
        /// </summary>
        /// <param name="node">The node to search.</param>
        /// <param name="sources">List of XPath selectors with attribute names for value.</param>
        /// <param name="def">The default value.</param>
        /// <returns>The node content or attribute value if found, otherwise <paramref name="def"/> will be returned.</returns>
        public static string FindSourceContent(this HtmlNode node, IDictionary<string, string> sources, string def = null)
        {
            // Iterate over the selectors
            foreach (var source in sources)
            {
                // Select node by a selector
                var result = node.SelectSingleNode(source.Key);

                // If node exists...
                if (result != null)
                {
                    // The value to be returned
                    var value = result.InnerText;

                    // If has an attribute we want and it's value is not empty...
                    if (source.Value != null)
                    {
                        // Set the attribute value
                        value = result.GetAttributeValue(source.Value, null);
                    }

                    // If we have the value...
                    if (!string.IsNullOrEmpty(value))
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
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static IList<string> SearchAttributes(this HtmlNode node, Regex regex)
        {
            var result = new List<string>();

            foreach (var attr in node.Attributes)
            {
                if (!string.IsNullOrWhiteSpace(attr.DeEntitizeValue))
                {
                    foreach (Match item in regex.Matches(attr.DeEntitizeValue))
                    {
                        result.Add(item.Value);
                    }
                }
            }

            return result;
        }

        public static IList<string> SearchAttributes(this HtmlNode node, Func<string, bool> func)
        {
            var result = new List<string>();

            foreach (var attr in node.Attributes)
            {
                if (!string.IsNullOrWhiteSpace(attr.DeEntitizeValue))
                {
                    if (func(attr.DeEntitizeValue))
                    {
                        result.Add(attr.DeEntitizeValue);
                    }
                }
            }

            return result;
        }
    }
}
