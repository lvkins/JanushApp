using System;
using System.Collections.ObjectModel;
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
        /// The interval for the product check.
        /// </summary>
        private TimeSpan _checkInterval;

        #endregion

        #region Public Properties

        /// <summary>
        /// If sound notification are enabled.
        /// </summary>
        public bool EnableSoundNotification { get; set; }

        /// <summary>
        /// Whether if the check interval time value should be randomized in order to prevent pulling the products data all together.
        /// </summary>
        public bool RandomizeCheckInterval { get; set; }


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

        /// <summary>
        /// The logs container.
        /// </summary>
        public ObservableCollection<Tuple<ProductViewModel, string, DateTime>> Logs { get; set; }

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
            DI.Application.ShowWindow<SettingsWindow>(this);
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public void Close()
        {
            DI.Application.CloseAllWindow<SettingsWindow>();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads the model components.
        /// </summary>
        private void Load()
        {
            // Get the user settings object
            var userSettings = DI.SettingsReader.Settings;

            // Set current values
            EnableSoundNotification = userSettings.SoundNotification;
            CheckInterval = userSettings.UpdateInterval;
        }

        #endregion
    }
}
