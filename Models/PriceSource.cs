namespace PromoSeeker
{
    /// <summary>
    /// 
    /// </summary>
    public class PriceSource : IPriceSource
    {
        /// <summary>
        /// 
        /// </summary>
        public string XPath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AttributeName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SourceText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Score { get; set; } = 0;

        public override string ToString()
        {
            return Price.ToString("N");
        }
    }
}
