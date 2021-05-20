// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

using Janush.Core.Localization;

namespace Janush
{
    /// <summary>
    /// A class containing details for the message dialog.
    /// </summary>
    public class MessageDialogBoxViewModel : BaseDialogWindowViewModel
    {
        #region Public Properties

        /// <summary>
        /// The text of the submit button.
        /// </summary>
        public string SubmitText { get; set; } = Strings.Ok;

        #endregion
    }
}
