using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.XPath;
using System;
using System.Collections.Generic;

namespace PromoSeeker.Core
{
    public static class HtmlNodeExtensions
    {
        /// <summary>
        /// Finds a certain node by XPath selector and returns it's text or the attribute value, if specified.
        /// </summary>
        /// <param name="node">The node to search.</param>
        /// <param name="sources">A dictionary of XPath queries along with the attribute names for value.</param>
        /// <param name="def">The default value.</param>
        /// <returns>The node content or attribute value if found, otherwise <paramref name="def"/> will be returned.</returns>
        public static string FindContentFromSource(this IElement node, IDictionary<string, string> sources, string def = null)
        {
            // Iterate over the selectors
            foreach (var source in sources)
            {
                // Select node by a selector
                var result = (HtmlElement)node.SelectSingleNode(source.Key);

                // If node exists...
                if (result != null)
                {
                    // The value to be returned
                    var value = result.TextContent;

                    // If has an attribute we want and it's value is not empty...
                    if (source.Value != null)
                    {
                        // Set the attribute value
                        value = result.GetAttribute(source.Value);
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
        /// Find closest <see cref="INode"/> that meets the given <paramref name="predicate"/> criteria.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="predicate"></param>
        /// <returns>Found <see cref="INode"/> instance if succeeded, otherwise <see langword="null"/>.</returns>
        public static IElement FindClosest(this IElement node, Func<IElement, bool> predicate, int maxDepth = 50)
        {
            // Current depth value
            var depth = 0;

            // Iterate until we have parent...
            while (node.ParentElement != null)
            {
                // Get parent node
                node = node.ParentElement;

                // If node meets given criteria...
                if (predicate(node))
                {
                    // We've succeeded.
                    return node;
                }

                // If maximum depth is reached...
                if (++depth >= maxDepth)
                {
                    // Break the loop
                    break;
                }
            }

            //Console.WriteLine($">> [HtmlNode::FindClosest] depth {depth}");

            // Node wasn't found - return null
            return null;
        }

        public static int StreamPosition(this IElement node)
        {
            var thisLength = node.OuterHtml.Length;
            var totalLength = 0;

            while (node.ParentElement != null)
            {
                node = node.ParentElement;
                totalLength += node.OuterHtml.Length;
            };

            var result = totalLength - thisLength;

            ;

            return result;
        }
    }
}
