using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromoSeeker.Core;
using System;

namespace PromoSeeker
{
    /// <summary>
    /// The dependency injection container.
    /// The dependency injection describes the pattern of passing dependencies to consuming services at instantiation.
    /// </summary>
    public static class DI
    {
        #region Public Properties

        /// <summary>
        /// The dependency injection service provider.
        /// </summary>
        public static IServiceProvider Provider { get; private set; }

        #region View Models

        /// <summary>
        /// A shortcut to access a singleton instance of the <see cref="ApplicationViewModel"/>.
        /// </summary>
        public static ApplicationViewModel Application => GetService<ApplicationViewModel>();

        /// <summary>
        /// A shortcut to access a singleton instance of the <see cref="AddPromotionViewModel"/>.
        /// </summary>
        public static AddProductViewModel AddPromotionViewModel => GetService<AddProductViewModel>();

        /// <summary>
        /// A shortcut to access a singleton instance of the <see cref="LogsViewModel"/>.
        /// </summary>
        public static SettingsViewModel SettingsViewModel => GetService<SettingsViewModel>();

        #endregion

        /// <summary>
        /// A shortcut to access a singleton instance of the <see cref="PromoSeeker.UIManager"/>.
        /// </summary>
        public static IUIManager UIManager => GetService<IUIManager>();

        /// <summary>
        /// A shortcut to access a singleton instance of the app configuration <see cref="IConfiguration"/>.
        /// </summary>
        public static IConfiguration Configuration => GetService<IConfiguration>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets up the all the dependencies container.
        /// </summary>
        public static void Setup()
        {
            // Create a service collection
            var serviceCollection = new ServiceCollection();

            // Add the application configuration
            serviceCollection.AddConfiguration(configure =>
            {
                configure.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                // Add environment variables
                // configure.AddEnvironmentVariables();
            });

            // Add view model services
            serviceCollection.AddViewModels();

            // Add application services
            serviceCollection.AddCoreServices();

            // Build and store the provider
            Provider = serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Gets an injected service of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the service to get from the provider.</typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            return Provider.GetService<T>();
        }

        #endregion
    }
}
