// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

using Janush.Core;
using System;

namespace Janush
{
    /// <summary>
    /// The design-time data model for a <see cref="NotificationItemViewModel"/>.
    /// </summary>
    public class NotificationItemDesignModel : NotificationItemViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model.
        /// </summary>
        public static NotificationItemDesignModel Instance = new NotificationItemDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public NotificationItemDesignModel()
        {
            Product = new ProductViewModel { DisplayName = "Dummy Product" };
            Message = "Product price has decreased!";
            Type = NotificationSubjectType.PriceDown;
            Date = DateTime.Now;
        }

        #endregion
    }
}
