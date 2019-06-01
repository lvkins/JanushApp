using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PromoSeeker.Core;
using System;

namespace PromoSeeker
{
    /// <summary>
    /// The extension methods for use in the <see cref="DI"/> composition.
    /// </summary>
    internal static class DICompositionExtensions
    {
        /// <summary>
        /// Builds the application configuration.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        public static void AddConfiguration(this IServiceCollection services, Action<IConfigurationBuilder> configure)
        {
            // Create the configuration builder
            var configurationBuilder = new ConfigurationBuilder();

            // Run action to configure
            configure(configurationBuilder);

            // Build the configuration and add to the services
            services.AddSingleton<IConfiguration>(configurationBuilder.Build());
        }


        /// <summary>
        /// Adds the view model services into the <paramref name="services"/>.
        /// </summary>
        /// <param name="services"></param>
        public static void AddViewModels(this IServiceCollection services)
        {
            // Bind to a single instance of application view model
            services.AddSingleton<ApplicationViewModel>();

            // Bind to a single instance of add product view model
            services.AddSingleton<AddProductViewModel>();

            // Bind to a single instance of settings view model
            services.AddSingleton<SettingsViewModel>();
        }


        /// <summary>
        /// Adds the core application services needed in the application. 
        /// </summary>
        /// <param name="services"></param>
        public static void AddCoreServices(this IServiceCollection services)
        {
            // Add a singleton user settings reader
            services.AddSingleton<IUserSettingsReader, SettingsReader>();

            // Add a singleton UI manager object
            services.AddSingleton<IUIManager, UIManager>();

            // Add a singleton web loader object
            services.AddSingleton<IWebLoader, WebLoader>();

            // Add a singleton logger
            services.AddSingleton<ILogger, Logger>(_ => new Logger("logs/application.log", new LoggerConfiguration
            {
                LogLevel = DI.Configuration.GetValue("Logging:LogLevel", LogLevel.Info),
            }));
        }
    }
}
