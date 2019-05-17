using PromoSeeker.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace PromoSeeker
{
    /// <summary>
    /// The view model for logs window.
    /// </summary>
    public class LogsViewModel : BaseViewModel, IWindowViewModel
    {
        #region Public Properties

        /// <summary>
        /// The logs container.
        /// </summary>
        public ObservableCollection<Tuple<ProductViewModel, string, DateTime>> Data { get; set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command to open the <see cref="LogsWindow"/>.
        /// </summary>
        public ICommand OpenCommand { get; set; }

        /// <summary>
        /// The command to close the <see cref="LogsWindow"/>.
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command to load the model components so it is ready to use.
        /// </summary>
        public ICommand LoadCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LogsViewModel()
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
            // Load logs
            Load();

            // Show
            DI.Application.ShowWindow<LogsWindow>(this);
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public void Close()
        {
            DI.Application.CloseAllWindow<LogsWindow>();
        }

        /// <summary>
        /// Loads the logs content.
        /// </summary>
        public void Load()
        {
            Debug.WriteLine("LogsViewModel.Load");

            // Create product changes log messages
            var productUpdates = DI.Application.Products
                .SelectMany(p =>
                {
                    // Create list of messages
                    var ret = new List<Tuple<ProductViewModel, string, DateTime>>();

                    // If product has name history...
                    if (p.NameHistory?.Any() == true)
                    {
                        // Add sequence changes
                        p.NameHistory.Aggregate((seed, next) =>
                        {
                            // Add name change message
                            ret.Add(new Tuple<ProductViewModel, string, DateTime>(p, $"Name has changed from {seed.Key} to {next.Key}.", seed.Value));
                            return next;
                        });

                        // Get last name
                        var nameLast = p.NameHistory.Last();

                        // Add change to current name message
                        ret.Add(new Tuple<ProductViewModel, string, DateTime>(p, $"Name has changed from {nameLast.Key} to {p.Name}", nameLast.Value));
                    }

                    // If product has price history...
                    if (p.PriceHistory?.Any() == true)
                    {
                        // Add sequence changes
                        p.PriceHistory.Aggregate((seed, next) =>
                        {
                            // Add name change message
                            ret.Add(new Tuple<ProductViewModel, string, DateTime>(p, $"Price has changed from {seed.Key} to {next.Key}.", seed.Value));
                            return next;
                        });

                        // Get last price
                        var priceLast = p.PriceHistory.Last();

                        // Add change to current price message
                        ret.Add(new Tuple<ProductViewModel, string, DateTime>(p, $"Price has changed from {priceLast.Key} to {p.DisplayPrice}", priceLast.Value));
                    }

                    // Return messages
                    return ret;
                })
                // Newest first
                .OrderByDescending(_ => _.Item3)
                // Take limited amount
                .Take(Consts.LOGS_LIMIT);

            // Initialize log messages container
            Data = new ObservableCollection<Tuple<ProductViewModel, string, DateTime>>(productUpdates);
        }

        #endregion
    }
}
