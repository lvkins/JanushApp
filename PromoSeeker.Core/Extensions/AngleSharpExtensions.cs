using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.XPath;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PromoSeeker.Core
{
    /// <summary>
    /// The extension methods for the <see cref="AngleSharp"/> 
    /// </summary>
    public static class AngleSharpExtensions
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
        /// <param name="predicate">The predicate to be used to compare nodes.</param>
        /// <param name="closestNode">Result of found closest node object.</param>
        /// <param name="reachDepth">Result of distance between nodes.</param>
        /// <returns><see cref="true"/> if the closest node was found, otherwise <see cref="false"/>.</returns>
        public static bool FindClosest(this IElement node, Func<IElement, bool> predicate, out IElement closestNode, out int reachDepth, int maxDepth = 50)
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
                    reachDepth = depth;
                    closestNode = node;
                    return true;
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
            reachDepth = -1;
            closestNode = null;
            return false;
        }

        /// <summary>
        /// Creates a unique selector path used to locate the element in the DOM.
        /// </summary>
        /// <param name="node">The starting node to create the selector path from.</param>
        /// <returns>The unique selector path for this element.</returns>
        public static string GetSelector(this IElement node)
        {
            // Initialize path
            var path = string.Empty;

            // If the current node is having an unique id property
            var hasId = false;

            do
            {
                // Set if node has id attribute set...
                hasId = !string.IsNullOrEmpty(node.Id);

                // Get parent element of the node
                var parent = node.ParentElement;

                // Always lowercase node name in the path
                var name = node.NodeName.ToLowerInvariant();

                // If node has id attribute...
                if (hasId)
                {
                    // Id is unique in the DOM, so we can use it to locate the element and skip other parents
                    name = "#" + node.Id;
                }
                // If node has siblings of the same type...
                else if (parent != null && !node.IsOnlyOfType())
                {
                    // Get node index in the parent node tree
                    var index = parent.Children.Where(_ => _.GetType() == node.GetType()).Index(node);

                    // Append nth child selector
                    name += $":nth-child({index + 1})";
                }

                // Set current parent
                node = parent;

                // Recreate selector path
                path = name + (!string.IsNullOrEmpty(path) ? ">" + path : "");
            } while (node?.ParentElement != null && !hasId);

            // Return generated selector
            return path;
        }

        /// <summary>
        /// Returns the first element within the document that matches the
        /// specified XPath query or CSS selector.
        /// </summary>
        /// <param name="node">A node to search within.</param>
        /// <param name="path">The XPath query or CSS selector.</param>
        /// <returns>The found element.</returns>
        public static IElement QuerySelectorOrXPath(this IElement node, string path)
        {
            // If path is a XPath query...
            if (path.StartsWith("/"))
            {
                // Return result of the XPath query
                return node.SelectSingleNode(path) as IElement;
            }

            // Otherwise assume CSS selector
            return node.QuerySelector(path);
        }
    }
}
