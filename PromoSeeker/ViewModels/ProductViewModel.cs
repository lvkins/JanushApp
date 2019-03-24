using System;
using System.Diagnostics;
using System.Windows.Input;

namespace PromoSeeker
{
    public class ProductViewModel
    {
        public string Name { get; set; }

        public decimal PriceCurrent { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public Uri Url { get; set; } = new Uri("https://google.com/some/path/dww.blah");

        public ICommand OpenCommand { get; set; }

        public ProductViewModel()
        {
            OpenCommand = new RelayCommand(() => Process.Start(Url.Scheme + "://" + Url.Host));
        }

        public void Load()
        {
            // TOOD: Load from the file
        }
    }
}
