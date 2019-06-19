using PromoSeeker.Core;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace PromoSeeker
{
    /// <summary>
    /// A view model representing the <see cref="SettingsWindow"/>.
    /// </summary>
    public class SettingsViewModel : BaseViewModel, IWindowViewModel
    {
        #region Private Members

        /// <summary>
        /// If products tracking should be restarted after window close.
        /// </summary>
        private bool _needTrackRestart;

        /// <summary>
        /// If sound notification are enabled.
        /// </summary>
        private bool _enableSoundNotification;

        /// <summary>
        /// Whether if the check interval time value should be randomized in order to prevent pulling the products data all together.
        /// </summary>
        private bool _randomizeCheckInterval;

        /// <summary>
        /// The interval for the product check.
        /// </summary>
        private TimeSpan _checkInterval;

        #endregion

        #region Public Properties

        /// <summary>
        /// If sound notification are enabled.
        /// </summary>
        public bool EnableSoundNotification
        {
            get => _enableSoundNotification;
            set
            {
                // Update value
                _enableSoundNotification = value;

                // Raise property changed event
                OnPropertyChanged(nameof(EnableSoundNotification));
            }
        }

        /// <summary>
        /// Whether if the check interval time value should be randomized in order to prevent pulling the products data all together.
        /// </summary>
        public bool RandomizeCheckInterval
        {
            get => _randomizeCheckInterval;
            set
            {
                // Update value
                _randomizeCheckInterval = value;

                // Raise property changed event
                OnPropertyChanged(nameof(RandomizeCheckInterval));
            }
        }

        /// <summary>
        /// The interval for the product check.
        /// </summary>
        public TimeSpan CheckInterval
        {
            get => _checkInterval;
            set
            {
                // Update value
                _checkInterval = value;

                // Raise property changed event
                OnPropertyChanged(nameof(CheckInterval));
                OnPropertyChanged(nameof(DisplayCheckInterval));
            }
        }

        /// <summary>
        /// Represents a readable value of the <see cref="CheckInterval"/>.
        /// </summary>
        public string DisplayCheckInterval
        {
            get
            {
                // Initialize string builder
                var sb = new StringBuilder();

                // Create a format string
                if (CheckInterval.Hours > 0)
                {
                    sb.Append(@"h\h\ ");
                }
                if (CheckInterval.Minutes > 0)
                {
                    sb.Append(@"m\m\ ");
                }
                if (CheckInterval.Seconds > 0)
                {
                    sb.Append(@"s\s\ ");
                }

                // Convert the timespan value
                return CheckInterval.ToString(sb.ToString());
            }
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command to open the <see cref="SettingsWindow"/>.
        /// </summary>
        public ICommand OpenCommand { get; set; }

        /// <summary>
        /// The command to close the <see cref="SettingsWindow"/>.
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command to load the model components so it is ready to use.
        /// </summary>
        public ICommand LoadCommand { get; set; }

        /// <summary>
        /// The command to save the settings inputs.
        /// </summary>
        public ICommand SaveCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SettingsViewModel()
        {
            // Create commands
            OpenCommand = new RelayCommand(Open);
            CloseCommand = new RelayCommand(Close);
            LoadCommand = new RelayCommand(Load);
            SaveCommand = new RelayCommand(Save);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens the window.
        /// </summary>
        public void Open()
        {
            // Load
            Load();

            // Show
            DI.Application.ShowWindow<SettingsWindow>(this, OnClose);
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public void Close()
        {
            // Call close callback
            OnClose();

            // Close window
            DI.Application.CloseAllWindow<SettingsWindow>();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads the model components.
        /// </summary>
        private void Load()
        {
            // Unhook so that property changed won't be called while populating view model properties
            PropertyChanged -= Settings_PropertyChanged;

            // Get the user settings object
            var userSettings = CoreDI.SettingsReader.Settings;

            // Set current values
            EnableSoundNotification = userSettings.SoundNotification;
            RandomizeCheckInterval = userSettings.RandomizeInterval;
            CheckInterval = userSettings.UpdateInterval;

            // Hook into the property changed event to listen for input changes
            PropertyChanged += Settings_PropertyChanged;
        }

        /// <summary>
        /// Saves the user input to the actual settings object.
        /// </summary>
        private void Save()
        {
            // Leave message
            Debug.WriteLine("SettingsViewModel::Save");

            // Get settings
            var settings = CoreDI.SettingsReader.Settings;

            // Flag for tracking restart if related settings was changed
            _needTrackRestart = _needTrackRestart || settings.RandomizeInterval != RandomizeCheckInterval ||
                settings.UpdateInterval != CheckInterval;

            // Update settings values
            settings.SoundNotification = EnableSoundNotification;
            settings.RandomizeInterval = RandomizeCheckInterval;
            settings.UpdateInterval = CheckInterval;

            // Save
            CoreDI.SettingsReader.Save();
        }

        /// <summary>
        /// Raised while a property within the view model changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            => Save();

        /// <summary>
        /// Called when window is closing.
        /// </summary>
        private void OnClose()
        {
            // If a tracking related-setting was changed...
            if (_needTrackRestart)
            {
                // Restart tracked products to apply the changes
                DI.Application.Products
                    .Where(_ => _.Tracked)
                    .ToList()
                    .ForEach(async product =>
                    {
                        // Stop tracking
                        await product.StopTrackingAsync();

                        // Start tracking
                        await product.StartTrackingAsync();
                    });
            }
        }

        #endregion
    }
}
