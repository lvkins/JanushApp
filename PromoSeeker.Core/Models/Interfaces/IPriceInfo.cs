using HtmlAgilityPack;

namespace PromoSeeker.Core
{
    public interface IPriceInfo
    {
        /// <summary>
        /// The type of price origin.
        /// </summary>
        PriceSourceType Source { get; set; }

        /// <summary>
        /// The node attribute name where the price is defined.
        /// </summary>
        string AttributeName { get; set; }

        /// <summary>
        /// The origin node where the price was located.
        /// </summary>
        HtmlNode SourceNode { get; set; }

        /// <summary>
        /// The price object.
        /// </summary>
        PriceValue Price { get; set; }
    }
}
