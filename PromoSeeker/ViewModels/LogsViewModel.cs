using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<Tuple<string, string, DateTime>> Data { get; set; }

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

            if (Data == null)
            {
                // Create product changes log messages
                var productUpdates = DI.Application.Products
                    .SelectMany(p =>
                    {
                        // Create list of messages
                        var ret = new List<Tuple<string, string, DateTime>>();

                        // If product has name history...
                        if (p.NameHistory?.Any() == true)
                        {
                            // Add sequence changes
                            p.NameHistory.Aggregate((seed, next) =>
                            {
                                // Add name change message
                                ret.Add(new Tuple<string, string, DateTime>(p.Name, $"Name has been changed from {seed.Key} to {next.Key}.", seed.Value));
                                return next;
                            });

                            // Get last name
                            var nameLast = p.NameHistory.Last();

                            // Add change to current name message
                            ret.Add(new Tuple<string, string, DateTime>(p.Name, $"Name has been changed from {nameLast.Key} to {p.Name}", nameLast.Value));
                        }

                        // If product has price history...
                        if (p.PriceHistory?.Any() == true)
                        {
                            // Add sequence changes
                            p.PriceHistory.Aggregate((seed, next) =>
                            {
                                // Add name change message
                                ret.Add(new Tuple<string, string, DateTime>(p.Name, $"Price has been changed from {seed.Key} to {next.Key}.", seed.Value));
                                return next;
                            });

                            // Get last price
                            var priceLast = p.PriceHistory.Last();

                            // Add change to current price message
                            ret.Add(new Tuple<string, string, DateTime>(p.Name, $"Price has been changed from {priceLast.Key} to {p.DisplayPrice}", priceLast.Value));
                        }

                        // Return messages
                        return ret;
                    })
                    // Newest first
                    .OrderByDescending(_ => _.Item3);

                // Initialize log messages container
                Data = new ObservableCollection<Tuple<string, string, DateTime>>(productUpdates);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens the window.
        /// </summary>
        public void Open()
        {
            DI.Application.ShowWindow<LogsWindow>(this);
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public void Close()
        {
            DI.Application.CloseAllWindow<LogsWindow>();
        }

        #endregion
    }
}
