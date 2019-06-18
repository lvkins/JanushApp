using System;
using System.Diagnostics;
using System.Windows.Input;

namespace PromoSeeker.Core
{
    /// <summary>
    /// A view model for the top shop out of all shops in the application.
    /// </summary>
    public class TopSellerViewModel
    {
        #region Public Properties

        /// <summary>
        /// The shop address.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Amount of products tracking in the shop.
        /// </summary>
        public int ProductCount { get; set; }

        /// <summary>
        /// Overall sales count in the shop.
        /// </summary>
        public int SaleCount { get; set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command to navigate to the shop URL.
        /// </summary>
        public ICommand NavigateCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TopSellerViewModel()
        {
            // Create command
            NavigateCommand = new RelayCommand(Navigate);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Navigates to the shop URL.
        /// </summary>
        public void Navigate() => Process.Start(Url.ToString());

        #endregion
    }
}
