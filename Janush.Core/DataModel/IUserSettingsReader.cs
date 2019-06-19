namespace Janush.Core
{
    /// <summary>
    /// Handles the user settings.
    /// </summary>
    public interface IUserSettingsReader
    {
        /// <summary>
        /// The current user setting instance.
        /// </summary>
        UserSettings Settings { get; set; }

        /// <summary>
        /// Initializes the <see cref="Settings"/> object by deserialization of the JSON settings file.
        /// </summary>
        void Load();

        /// <summary>
        /// Serializes the <see cref="Settings"/> object into a JSON string and saves to the settings file.
        /// </summary>
        void Save();
    }
}
