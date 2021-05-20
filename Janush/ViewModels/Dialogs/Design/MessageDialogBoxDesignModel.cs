// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

namespace Janush
{
    /// <summary>
    /// The design-time data model for a <see cref="MessageDialogBoxViewModel"/>.
    /// </summary>
    public class MessageDialogBoxDesignModel : MessageDialogBoxViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model.
        /// </summary>
        public static MessageDialogBoxDesignModel Instance = new MessageDialogBoxDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MessageDialogBoxDesignModel()
        {
            Title = "Message Dialog";
            Message = "Hello, design time!";
        }

        #endregion
    }
}
