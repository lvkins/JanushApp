﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace Janush.Core
{
    /// <summary>
    /// The dependency injection container for the class library.
    /// The dependency injection describes the pattern of passing dependencies to consuming services at instantiation.
    /// </summary>
    public class CoreDI
    {
        #region Public Properties

        /// <summary>
        /// The dependency injection service provider.
        /// </summary>
        public static IServiceProvider Provider { get; set; }

        /// <summary>
        /// A shortcut to access a instance of the <see cref="Core.Logger"/>.
        /// </summary>
        public static ILogger Logger => GetService<ILogger>();

        /// <summary>
        /// A shortcut to access a instance of the <see cref="IUserSettingsReader"/>.
        /// </summary>
        public static IUserSettingsReader SettingsReader => GetService<IUserSettingsReader>();

        /// <summary>
        /// A shortcut to access a instance of the <see cref="IWebLoader"/>.
        /// </summary>
        public static IWebLoader WebLoader => GetService<IWebLoader>();

        /// <summary>
        /// A shortcut to access a instance of the <see cref="IEmailDispatcher"/>.
        /// </summary>
        public static IEmailDispatcher EmailDispatcher => GetService<IEmailDispatcher>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets an injected service of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the service to get from the provider.</typeparam>
        /// <exception cref="InvalidOperationException"><see cref="Provider"/> is not set.</exception>
        /// <returns></returns>
        public static T GetService<T>()
        {
            // If provider wasn't registered...
            if (Provider == null)
            {
                // Raise exception
                throw new InvalidOperationException("Provider wasn't registered");
            }

            // Return service
            return Provider.GetService<T>();
        }

        #endregion
    }
}
