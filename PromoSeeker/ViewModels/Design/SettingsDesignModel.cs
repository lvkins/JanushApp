﻿using System;
using System.Collections.ObjectModel;

namespace PromoSeeker
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
