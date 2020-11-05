using Janush.Core.Localization;
using System;
using System.Windows.Input;

namespace Janush
{
    /// <summary>
    /// A class containing details for a confirmation message box dialog.
    /// </summary>
    public class ConfirmDialogBoxViewModel : BaseDialogWindowViewModel
    {
        #region Public Properties

        /// <summary>
        /// The user confirmation response.
        /// </summary>
        public bool Response { get; set; }

        /// <summary>
        /// The text to use for the OK button.
        /// </summary>
        public string OkText { get; set; } = Strings.Ok;

        /// <summary>
        /// The text to use for the Cancel button.
        /// </summary>
        public string CancelText { get; set; } = Strings.Cancel;

        /// <summary>
        /// The action to execute once confirm dialog response was positive.
        /// </summary>
        public Action OnAccept { get; set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// Proceeds with given input.
        /// </summary>
        public ICommand AcceptCommand { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ConfirmDialogBoxViewModel()
        {
            AcceptCommand = new RelayParamCommand((param) =>
            {
                // Set the response
                Response = true;

                // Call dialog close command
                if (param is ICommand command && command.CanExecute(null))
                {
                    command.Execute(null);
                }

                // Invoke accept callback
                OnAccept?.Invoke();
            });
        }

        #endregion
    }
}
