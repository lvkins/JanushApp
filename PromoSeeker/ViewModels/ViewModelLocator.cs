using System.ComponentModel;
using System.Windows;

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
        public static ApplicationViewModel ApplicationViewModel
            = DesignerProperties.GetIsInDesignMode(new DependencyObject())
            // If we are in design time, just return new design instance
            ? new ApplicationDesignModel()
            // Otherwise return dependency
            : DI.Application;
    }
}
