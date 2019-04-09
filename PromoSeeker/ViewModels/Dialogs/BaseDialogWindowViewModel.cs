namespace PromoSeeker
{
    /// <summary>
    /// A base class for the dialog boxes.
    /// </summary>
    public class BaseDialogWindowViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The type of the dialog box.
        /// </summary>
        private DialogBoxType _type = DialogBoxType.None;

        #endregion

        #region Public Properties

        /// <summary>
        /// Dialog popup title.
        /// </summary>
        public string Title { get; set; } = "Heads up!";

        /// <summary>
        /// The message to be displayed.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The type of the dialog box.
        /// </summary>
        public DialogBoxType Type
        {
            get => _type;
            set
            {
                _type = value;

                // Use enum type value
                // TODO: Use localization
                Title = Type.ToString();
            }
        }

        /// <summary>
        /// The dialog minimum width.
        /// </summary>
        public int WindowMinimumWidth = 250;

        /// <summary>
        /// The dialog minimum height.
        /// </summary>
        public int WindowMinimumHeight = 150;

        #endregion
    }
}
