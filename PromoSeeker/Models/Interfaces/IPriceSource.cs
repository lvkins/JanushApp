using HtmlAgilityPack;

namespace PromoSeeker
{
    public interface IPriceSource
    {
        /// <summary>
        /// 
        /// </summary>
        PriceSourceType Source { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string AttributeName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        HtmlNode SourceNode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        PriceValue Price { get; set; }
    }
}
