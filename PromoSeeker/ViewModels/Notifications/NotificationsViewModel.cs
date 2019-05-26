using PromoSeeker.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace PromoSeeker
{
    /// <summary>
    /// A view model for the notifications list.
    /// </summary>
    public class NotificationsViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// Whether there are new notifications available.
        /// </summary>
        private bool _new;

        #endregion

        #region Public Properties

        /// <summary>
        /// A list of notifications.
        /// </summary>
        public List<NotificationItemViewModel> Items { get; set; }

        /// <summary>
        /// Whether there are new notifications available.
        /// </summary>
        public bool New
        {
            get => _new;
            set
            {
                // Update value
                _new = value;

                // Raise property changed event
                OnPropertyChanged(nameof(New));
            }
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command to load the notifications for the user.
        /// </summary>
        public ICommand LoadCommand { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public NotificationsViewModel()
        {
            // Create commands
            LoadCommand = new RelayCommand(Load);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads recent notifications to display.
        /// </summary>
        public void Load()
        {
            // Flag no new notifications are available
            New = false;

            // Whether this is a first load
            var firstLoad = Items == null;

            // Create product changes log messages
            Items = DI.Application.Products
                .SelectMany(p =>
                {
                    // Create list of messages
                    var ret = new List<NotificationItemViewModel>();

                    // If product has name history...
                    if (p.NameHistory?.Any() == true)
                    {
                        // Add sequence changes
                        p.NameHistory.Aggregate((seed, next) =>
                        {
                            // Add name change message
                            ret.Add(new NotificationItemViewModel
                            {
                                Product = p,
                                Date = seed.Value,
                                IsNew = !firstLoad,
                                Type = NotificationType.NameChange,
                                Message = $"Name has changed from {seed.Key} to {next.Key}.",
                            });
                            return next;
                        });

                        // Get last name
                        var nameLast = p.NameHistory.Last();

                        // Add change to current name message
                        ret.Add(new NotificationItemViewModel
                        {
                            Product = p,
                            IsNew = !firstLoad,
                            Date = nameLast.Value,
                            Type = NotificationType.NameChange,
                            Message = $"Name has changed from {nameLast.Key} to {p.Name}",
                        });
                    }

                    // Prepare and format price history
                    var priceHistory = p.PriceHistory?.Select(_ => new
                    {
                        Price = _.Key,
                        Date = _.Value,
                        IsNew = !firstLoad,
                        PriceFormatted = _.Key.ToString("C2", p.Culture),
                    });

                    // If product has price history...
                    if (priceHistory?.Any() == true)
                    {
                        // Price percentage change
                        decimal change;

                        // Add sequence changes
                        priceHistory.Aggregate((seed, next) =>
                        {
                            // Get percentage change
                            change = 1 - Math.Min(seed.Price, next.Price) / Math.Max(seed.Price, next.Price);

                            // Add name change message
                            ret.Add(new NotificationItemViewModel
                            {
                                Product = p,
                                Date = seed.Date,
                                IsNew = !firstLoad,
                                Type = seed.Price < next.Price ? NotificationType.PriceUp : NotificationType.PriceDown,
                                Message = $"Price has {(seed.Price < next.Price ? "increased" : "decreased")} from {seed.PriceFormatted} to {next.PriceFormatted} ({change.ToString("P")} change).",
                            });

                            return next;
                        });

                        // Get last price
                        var priceLast = priceHistory.Last();

                        // Get percentage change
                        change = 1 - Math.Min(priceLast.Price, p.PriceCurrent) / Math.Max(priceLast.Price, p.PriceCurrent);

                        // Add change to current price message
                        ret.Add(new NotificationItemViewModel
                        {
                            Product = p,
                            IsNew = !firstLoad,
                            Date = priceLast.Date,
                            Type = priceLast.Price < p.PriceCurrent ? NotificationType.PriceUp : NotificationType.PriceDown,
                            Message = $"Price has {(priceLast.Price < p.PriceCurrent ? "increased" : "decreased")} from {priceLast.PriceFormatted} to {p.DisplayPrice} ({change.ToString("P")} change).",
                        });
                    }

                    // Return messages
                    return ret;
                })
                // Newest first
                .OrderByDescending(_ => _.Date)
                // Take limited amount
                .Take(Consts.LOGS_LIMIT)
                // Make a list
                .ToList();

            // If there's a new notification, flag it
            New = Items.Any(_ => _.IsNew);

            // Raise property changed event
            OnPropertyChanged(nameof(Items));
        }

        #endregion
    }
}
