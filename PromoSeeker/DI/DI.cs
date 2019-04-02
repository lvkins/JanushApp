using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromoSeeker.Core;
using System;
using System.IO;
using System.Reflection;

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

        internal static ApplicationViewModel Application => GetService<ApplicationViewModel>();
        internal static AddProductViewModel AddPromotionViewModel => GetService<AddProductViewModel>();

        /// <summary>
        /// A shortcut to access a singleton instance of the <see cref="Core.UserSettings"/>.
        /// </summary>
        public static UserSettings UserSettings => GetService<IUserSettingsReader>().Settings;

        /// <summary>
        /// A shortcut to access a singleton instance of the <see cref="PromoSeeker.Logger"/>.
        /// </summary>
        public static ILogger Logger => GetService<ILogger>();

        /// <summary>
        /// A shortcut to access a singleton instance of the <see cref="PromoSeeker.UIManager"/>.
        /// </summary>
        public static IUIManager UIManager => GetService<IUIManager>();

        /// <summary>
        /// A shortcut to access a singleton instance of the app configuration <see cref="IConfiguration"/>.
        /// </summary>
        public static IConfiguration Configuration => GetService<IConfiguration>();

        /// <summary>
        /// A shortcut to access a singleton instance of the <see cref="System.Random"/>.
        /// </summary>
        //public static Random Random => Provider.GetService<Random>();

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

            #region Add application services to be used

            // Add view model services
            serviceCollection.AddViewModels();

            // Add a singleton user settings reader.
            serviceCollection.AddSingleton<IUserSettingsReader, SettingsReader>();

            // Add a singleton UI manager object
            serviceCollection.AddSingleton<IUIManager, UIManager>();

            // Add a singleton logger.
            serviceCollection.AddSingleton<ILogger, Logger>(_ => new Logger("logs/application.log", new LoggerConfiguration
            {
                LogLevel = Configuration.GetValue("Logging:LogLevel", LogLevel.Info),
            }));

            // Add singleton Random
            //serviceCollection.AddSingleton<Random>(); 

            #endregion

            // Build and store the provider
            Provider = serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// Gets an injected service of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the service to get.</typeparam>
        /// <returns></returns>
        public static T GetService<T>()
        {
            return Provider.GetService<T>();
        }

        #endregion
    }
}
