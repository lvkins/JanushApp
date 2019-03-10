using HtmlAgilityPack;

namespace PromoSeeker
{
    /// <summary>
    /// 
    /// </summary>
    public enum PriceSourceType
    {
        /// <summary>
        /// 
        /// </summary>
        PriceSourceAttribute,

        /// <summary>
        /// 
        /// </summary>
        PriceSourceJavascript,

        /// <summary>
        /// 
        /// </summary>
        PriceSourceText,
    }

    /// <summary>
    /// 
    /// </summary>
    public class PriceSource : IPriceSource
    {
        /// <summary>
        /// 
        /// </summary>
        public PriceSourceType Source { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HtmlNode SourceNode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PriceValue Price { get; set; }

        public override string ToString()
        {
            return Price.Decimal.ToString("N");
        }
    }
}
