namespace PromoSeeker
{
    /// <summary>
    /// The design-time data model for a <see cref="MessageDialogViewModel"/>.
    /// </summary>
    public class MessageDialogBoxDesignModel : MessageDialogViewModel
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
