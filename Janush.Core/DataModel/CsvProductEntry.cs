using System;

namespace Janush
{
    /// <summary>
    /// A class representing CSV entry for a product.
    /// </summary>
    public class CsvProductEntry
    {
        /// <summary>
        /// History entry date.
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// A product name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// A product price.
        /// </summary>
        public string Price { get; set; }
    }
}
