// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

namespace Janush
{
    /// <summary>
    /// The design-time data model for a <see cref="AddProductViewModel"/>.
    /// </summary>
    public class AddProductDesignModel : AddProductViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model.
        /// </summary>
        public static AddProductDesignModel Instance = new AddProductDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AddProductDesignModel()
        {
            StepTwo = false;
            Status = "Heads up! This is a design time message!";
        }

        #endregion
    }
}
