using System;

namespace PromoSeeker
{
    public class ProductViewModel
    {
        public string Name { get; set; }

        public decimal PriceCurrent { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public void Load()
        {
            // TOOD: Load from the file
        }
    }
}
