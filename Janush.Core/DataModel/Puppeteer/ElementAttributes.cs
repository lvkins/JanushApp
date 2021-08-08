namespace Janush.Core
{
    /// <summary>
    /// An object containing evaluated attribute data.
    /// </summary>
    public class ElementAttributes
    {
        /// <summary>
        /// The attribute name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The attribute value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Name}:{Value}";
    }
}
