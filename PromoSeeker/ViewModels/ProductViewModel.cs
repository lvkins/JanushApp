using System;
using System.Diagnostics;
using System.Windows.Input;
using PromoSeeker.Core;

namespace PromoSeeker
{
    /// <summary>
    /// The view model representing the product in the tracker.
    /// </summary>
    public class ProductViewModel
    {
        #region Public Properties

        public string Name { get; set; }

        public decimal PriceCurrent { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public Uri Url { get; set; } = new Uri("https://google.com/some/path/dww.blah");

        /// <summary>
        /// The product data.
        /// </summary>
        public Product Product { get; internal set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command for opening the product in the browser.
        /// </summary>
        public ICommand NavigateCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProductViewModel()
        {
            NavigateCommand = new RelayCommand(() => Process.Start(Url.Scheme + "://" + Url.Host));
        }

        #endregion

        /// <summary>
        /// Loads the product.
        /// </summary>
        public void Load()
        {
            // TOOD: Load from the file
        }

        /// <summary>
        /// Saves the product to the file.
        /// </summary>
        public void Save()
        {
            // TODO: Save to the file
        }
    }
}
