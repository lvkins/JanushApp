using HtmlAgilityPack;

namespace PromoSeeker.Core
{
    /// <summary>
    /// The types of the price origin.
    /// </summary>
    public enum PriceSourceType
    {
        /// <summary>
        /// A price that is coming from an attribute.
        /// </summary>
        PriceSourceAttribute,

        /// <summary>
        /// A price that is coming from the Javascript parsing.
        /// </summary>
        PriceSourceJavascript,

        /// <summary>
        /// A price that is coming from the raw node text.
        /// </summary>
        PriceSourceText,
    }

    /// <summary>
    /// An object containing all the key informations about the price and it's origin.
    /// </summary>
    public class PriceInfo : IPriceInfo
    {
        /// <summary>
        /// The type of price origin.
        /// </summary>
        public PriceSourceType Source { get; set; }

        /// <summary>
        /// The node attribute name where the price is defined.
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// The origin node where the price was located.
        /// </summary>
        public HtmlNode SourceNode { get; set; }

        /// <summary>
        /// The price object.
        /// </summary>
        public PriceValue Price { get; set; }
    }
}
