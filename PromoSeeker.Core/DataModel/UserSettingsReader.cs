using Newtonsoft.Json;
using System;
using System.IO;

namespace PromoSeeker.Core
{
    /// <summary>
    /// Handles the user settings.
    /// </summary>
    public class SettingsReader : IUserSettingsReader
    {
        #region Private Members

        /// <summary>
        /// The settings file path.
        /// </summary>
        private static readonly string SettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Consts.APP_TITLE, @"Settings.json");

        /// <summary>
        /// The JSON serializer settings to be used to proceed the settings serialization.
        /// </summary>
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            // Allow classes deserialization using their non-public constructor.
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            // Ignore serialization of null values
            NullValueHandling = NullValueHandling.Ignore,
        };

        #endregion

        #region Public Properties

        /// <summary>
        /// The current user setting instance.
        /// </summary>
        public UserSettings Settings { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// A default constructor
        /// </summary>
        public SettingsReader()
        {
            // Loads user settings
            Load();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the <see cref="Settings"/> object by deserializing the JSON settings file.
        /// </summary>
        public void Load()
        {
            // If the file doesn't exists...
            if (!File.Exists(SettingsPath))
            {
                // Use default
                Settings = new UserSettings();
                return;
            }

            Settings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(SettingsPath), JsonSerializerSettings);
        }

        /// <summary>
        /// Serializes the <see cref="Settings"/> object into a JSON string and saves to the settings file.
        /// </summary>
        public void Save()
        {
            // If settings are not initialized...
            if (Settings == null)
            {
                // Do nothing
                return;
            }

            // Create settings directory, if not exists (there's an internal check for that)
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));

            // Serialize user settings
            var json = JsonConvert.SerializeObject(Settings, Formatting.Indented, JsonSerializerSettings);

            // Write to file
            File.WriteAllText(SettingsPath, json);
        }

        #endregion
    }


}
