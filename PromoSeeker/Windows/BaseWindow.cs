using System;
using System.Linq;
using System.Windows;

namespace PromoSeeker
{
    internal class BaseWindow : Window
    {
        #region Private Members

        /// <summary>
        /// The view model for this window.
        /// </summary>
        private BaseViewModel _viewModel;

        #endregion

        #region Public Properties

        /// <summary>
        /// The view model for this window.
        /// </summary>
        public BaseViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                // Set value
                _viewModel = value;

                // Set window data context
                DataContext = _viewModel;
            }
        }

        #endregion
    }
}
