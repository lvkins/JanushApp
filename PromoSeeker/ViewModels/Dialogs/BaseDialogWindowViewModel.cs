namespace PromoSeeker
{
    public class BaseDialogWindowViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// Dialog popup title.
        /// </summary>
        public string Title { get; set; } = "Information"; // TODO: use localization

        /// <summary>
        /// The message to be displayed.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The type of the dialog box.
        /// </summary>
        public DialogBoxType Type { get; set; } = DialogBoxType.None;

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
