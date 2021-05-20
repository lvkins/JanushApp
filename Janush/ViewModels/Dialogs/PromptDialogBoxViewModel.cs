// Copyright(c) Łukasz Szwedt. All rights reserved.
// Licensed under the MIT license.

using Janush.Core.Localization;

namespace Janush
{
    /// <summary>
    /// A class containing details for the message dialog.
    /// </summary>
    public class PromptDialogBoxViewModel : BaseDialogWindowViewModel
    {
        #region Public Properties

        /// <summary>
        /// The text of the submit button.
        /// </summary>
        public string SubmitText { get; set; } = Strings.Ok;

        /// <summary>
        /// The text of the cancel button.
        /// </summary>
        public string CancelText { get; set; } = Strings.Cancel;

        /// <summary>
        /// Whether the prompt message dialog can be canceled.
        /// </summary>
        public bool Cancelable { get; set; }

        /// <summary>
        /// The placeholder of the input control.
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        /// The user input.
        /// </summary>
        public string Input { get; set; }

        /// <summary>
        /// Whether if spell check functionality should be enabled for the input control.
        /// </summary>
        public bool SpellCheck { get; set; }

        /// <summary>
        /// The maximum allowed length of the user input.
        /// </summary>
        public int MaxLength { get; set; } = 128;

        #endregion
    }
}
