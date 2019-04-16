namespace PromoSeeker
{
    /// <summary>
    /// A view model locator object that is used to locate various view models within the XAML files.
    /// </summary>
    public static class ViewModelLocator
    {
        /// <summary>
        /// A singleton instance of the <see cref="ApplicationViewModel"/>.
        /// </summary>
        public static ApplicationViewModel ApplicationViewModel => DI.Application;
    }
}
