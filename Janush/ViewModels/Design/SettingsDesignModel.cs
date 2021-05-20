// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.ObjectModel;

namespace Janush
{
    /// <summary>
    /// The design-time data model for a <see cref="SettingsViewModel"/>.
    /// </summary>
    public class SettingsDesignModel : SettingsViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model.
        /// </summary>
        public static SettingsDesignModel Instance = new SettingsDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SettingsDesignModel()
        {
            // Set properties
            EnableSoundNotification = true;
            CheckInterval = TimeSpan.FromMinutes(10);
        }

        #endregion
    }
}
