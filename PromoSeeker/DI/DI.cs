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

        /// <summary>
        /// A shortcut to access a singleton instance of the <see cref="Core.UserSettings"/>.
        /// </summary>
        public static UserSettings UserSettings => Provider.GetService<IUserSettingsReader>().Settings;

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

            // Add services to be used in our application

            // A singleton user settings reader.
            serviceCollection.AddSingleton<IUserSettingsReader, SettingsReader>();

            // Add singleton Random
            //serviceCollection.AddSingleton<Random>();

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
