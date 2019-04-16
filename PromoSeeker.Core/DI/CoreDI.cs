namespace PromoSeeker.Core
{
    /// <summary>
    /// The dependency injection container.
    /// The dependency injection describes the pattern of passing dependencies to consuming services at instantiation.
    /// </summary>
    public class CoreDI
    {
        /// <summary>
        /// The file logger singleton instance.
        /// </summary>
        public static ILogger Logger { get; set; }
    }
}
