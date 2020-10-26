using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace Janush.Core
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
        /// The backup directory absolute path.
        /// </summary>
        private static readonly string BackupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Consts.APP_TITLE, "Backup");

        /// <summary>
        /// The settings backup file path.
        /// </summary>
        //private static readonly string BackupSettingsPath = Path.Combine(BackupDir, @"Settings.back");

        /// <summary>
        /// The number of days to keep the logs.
        /// </summary>
        private static readonly TimeSpan LogRotateInterval = TimeSpan.FromDays(7);

        /// <summary>
        /// The JSON serializer settings to be used to proceed the settings serialization.
        /// </summary>
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            // Allow classes deserialization using their non-public constructor.
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            // Ignore serialization of null values
            NullValueHandling = NullValueHandling.Ignore,
            // Use references handling for object
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };

        /// <summary>
        /// A lock for the settings access to keep it thread-safe.
        /// </summary>
        private readonly object _settingsLock = new object();

        #endregion

        #region Public Properties

        /// <summary>
        /// The current user setting instance.
        /// </summary>
        public UserSettings Settings { get; set; }

        /// <summary>
        /// If a backup of the current settings file should be made before saving a new file.
        /// </summary>
        public bool BackupSettings { get; } = true;

        #endregion

        #region Constructor

        /// <summary>
        /// A default constructor
        /// </summary>
        public SettingsReader()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the <see cref="Settings"/> object by deserialization of the JSON settings file.
        /// </summary>
        public void Load()
        {
            // Lock the settings for thread safety
            lock (_settingsLock)
            {
                // If the file doesn't exists or unable to deserialize...
                if (!File.Exists(SettingsPath) || (Settings = JsonConvert.DeserializeObject<UserSettings>(
                    File.ReadAllText(SettingsPath), JsonSerializerSettings)) == null)
                {
                    // Temp
                    Debugger.Break();

                    // Use default
                    Settings = new UserSettings();
                    return;
                }
            }
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

            // Lock the settings for thread safety
            lock (_settingsLock)
            {
                // Create settings directory, if not exists (there's an internal check for that)
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));

                // If backup should be made...
                if (BackupSettings && File.Exists(SettingsPath))
                {
                    // Ensure backup directory
                    Directory.CreateDirectory(BackupDir);

                    // Get current date
                    var now = DateTime.Now;

                    // Copy settings
                    File.Copy(SettingsPath, Path.Combine(BackupDir, $"Settings.{now.ToFileTime()}"), true);

                    // Cleanup logs
                    if (LogRotateInterval.Seconds > 0)
                    {
                        foreach (var filePath in Directory.GetFiles(BackupDir))
                        {
                            // Get timespan
                            var dif = now - File.GetCreationTime(filePath);
                            if (dif >= LogRotateInterval)
                            {
                                CoreDI.Logger.Trace($"Log rotate: ${filePath}");

                                // Remove backup file
                                File.Delete(filePath);
                            }
                        }
                    }
                }

                // Serialize user settings
                var json = JsonConvert.SerializeObject(Settings, Formatting.Indented, JsonSerializerSettings);

                // Write to file
                File.WriteAllText(SettingsPath, json);
            }
        }

        #endregion
    }


}
