// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

namespace Janush
{
    /// <summary>
    /// The design-time data model for a <see cref="PromptDialogBoxViewModel"/>.
    /// </summary>
    public class PromptDialogBoxDesignModel : PromptDialogBoxViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model.
        /// </summary>
        public static PromptDialogBoxDesignModel Instance = new PromptDialogBoxDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public PromptDialogBoxDesignModel()
        {
            Title = "Prompt Dialog";
            Type = DialogBoxType.Information;
            Message = "Heads up! This is a design time message!";
        }

        #endregion
    }
}
