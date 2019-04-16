using System;

namespace PromoSeeker.Core
{
    public class Tracker
    {
        private readonly string[] URLS = {
            @"https://www.zalando.pl/polo-ralph-lauren-golf-fairway-czapka-z-daszkiem-po742l00d-k11.html",
            @"https://allegro.pl/oferta/nawiewnik-wrebowy-2-szt-kpl-zyj-zdrowo-bez-smogu-7705477010",
            @"https://mediamarkt.pl/rtv-i-telewizory/telewizor-lg-55uk7550mla",
            @"https://www.x-kom.pl/p/415102-smartfon-telefon-huawei-p20-pro-dual-sim-128gb-granatowy.html",
            @"https://www.amazon.com/Waste-King-L-2600-Garbage-Disposal/dp/B0014X7B54/",
            @"https://www.zooplus.pl/shop/psy/karma_dla_psa_sucha/brit/brit_premium/474101",
            @"https://www.olx.pl/oferta/sony-a6000-aparat-bezlusterkowiec-idelany-18-55-i-55-2-8-CID99-IDyJWKZ.html#3c7258dcef",
            @"https://www.mediaexpert.pl/aparaty-z-wymienna-optyka-bezlustrowce-/aparat-olympus-e-m10-mark-ii-czarny-plus-obiektyw-ez-14-42-mm-iir,id-706986",
        };

        #region Public Events

        /// <summary>
        /// 
        /// </summary>
        public event Action<Product> OnPriceChanged = (_) => { };

        #endregion

        #region Constructor

        public Tracker()
        {

        }

        #endregion

        internal void Work()
        {
            foreach (var url in URLS)
            {
                System.Console.WriteLine($"Currently seeking promo: {url}");
                var promo = new Product(url);
                ;
            }

            //doc.Save("doc1.html");
        }

        public void CheckPrices()
        {

        }

        public static void AddProduct(Product product)
        {

        }
    }
}
