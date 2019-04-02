using PromoSeeker.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PromoSeeker
{
    /// <summary>
    /// The view model for <see cref="AddProductWindow"/>.
    /// </summary>
    public class AddProductViewModel : BaseViewModel, IWindowViewModel
    {
        #region Private Members

        /// <summary>
        /// If we are currently adding a product.
        /// </summary>
        private bool _isBusy;

        /// <summary>
        /// Whether the step two overlay is visible.
        /// </summary>
        private bool _stepTwoVisible;

        #endregion

        #region Public Properties

        /// <summary>
        /// The product name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Product full URL.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The price selector 
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// Any status message about adding a product that is displayed to the user.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The regions to select the currency from.
        /// </summary>
        public IEnumerable<RegionInfo> Regions { get; } = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
            // Select the region info of the culture
            .Select(_ => new RegionInfo(_.Name))
            // Ignore multiple regions with the same id
            .GroupBy(_ => _.GeoId)
            .Select(_ => _.First())
            // Prevent same regions
            .Distinct()
            // Order ascending by region name
            .OrderBy(_ => _.DisplayName)
            .ToList();

        /// <summary>
        /// The current user region informations.
        /// </summary>
        public RegionInfo UserRegion { get; set; } = new RegionInfo(CultureInfo.CurrentUICulture.Name); // Binding is not working when using RegionInfo.CurrentRegion;

        /// <summary>
        /// If we are currently adding a product.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                // Update value
                _isBusy = value;

                // Raise property changed event
                OnPropertyChanged(nameof(IsBusy));
            }
        }

        /// <summary>
        /// Whether the step two overlay is visible.
        /// </summary>
        public bool StepTwo
        {
            get => _stepTwoVisible;
            set
            {
                // Update value
                _stepTwoVisible = value;

                // Raise property changed event
                OnPropertyChanged(nameof(StepTwo));
            }
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command to open the <see cref="AddProductWindow"/>.
        /// </summary>
        public ICommand OpenCommand { get; set; }

        /// <summary>
        /// The command to close the <see cref="AddProductWindow"/>.
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command to add the promotion automatically.
        /// </summary>
        public ICommand AddAutoCommand { get; set; }

        /// <summary>
        /// The command to add the promotion manually.
        /// </summary>
        public ICommand AddManuallyCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AddProductViewModel()
        {
            // Create commands
            OpenCommand = new RelayCommand(Open);
            CloseCommand = new RelayCommand(Close);
            AddAutoCommand = new RelayCommand(async () => await AddAutoProductAsync());
            AddManuallyCommand = new RelayCommand(async () => await AddProductManuallyAsync());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Opens a window.
        /// </summary>
        public void Open() => DI.Application.ShowWindow<AddProductWindow>(this);

        /// <summary>
        /// Closes a window.
        /// </summary>
        public void Close() => DI.Application.CloseWindow<AddProductWindow>();

        /// <summary>
        /// Adds a new product by specified <see cref="Url"/> to the tracker by automatically detecting it's properties.
        /// </summary>
        /// <returns></returns>
        public async Task AddAutoProductAsync()
        {
            // TODO: Check if the product was already added

            /*
             * 1. Get required product parameters (name, price, culture info (currency)).
             * 2. Show results to the user in the inputs,
             * 3. If some parameters were incorrect, give the user right to correct them,
             * 4. Price correction can be made via the dropdown of already detected prices - or via user specified xpath selector.
             * 5. After product is set - verify one more time,
             * 6. Store product.
             */

            IsBusy = true;

            await Task.Delay(TimeSpan.FromSeconds(5));

            StepTwo = true;

            //var product = new Product(Url);

            //var result = await product.LoadAsync();
            IsBusy = false;
        }

        /// <summary>
        /// Adds a new product to the tracker by manually user specified properties.
        /// </summary>
        /// <returns></returns>
        public async Task AddProductManuallyAsync()
        {
            IsBusy = true;

            await DI.UIManager.MessageAsync(new DialogWindowViewModel
            {
                Title = "Works like a charm!"
            });

            IsBusy = false;
        }

        #endregion
    }
}
