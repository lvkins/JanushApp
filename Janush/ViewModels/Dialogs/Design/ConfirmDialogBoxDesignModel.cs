﻿// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

namespace Janush
{
    /// <summary>
    /// The design-time data model for a <see cref="ConfirmDialogBoxViewModel"/>.
    /// </summary>
    public class ConfirmDialogBoxDesignModel : ConfirmDialogBoxViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model.
        /// </summary>
        public static ConfirmDialogBoxDesignModel Instance = new ConfirmDialogBoxDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ConfirmDialogBoxDesignModel()
        {
            Title = "Confirm Message Dialog";
            Message = "Hello from the design!";
        }

        #endregion
    }
}
