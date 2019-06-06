using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser.Tokens;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PromoSeeker.Core
{
    public static class AngleSharpTest
    {
        public static async void OneAsync()
        {
            var doc = await WebLoader.LoadAsync("https://allegro.pl/oferta/komoda-do-salonu-loco-w-stylu-skandynawskim-dab-7920821262");
            var main = doc.QuerySelector(".main-wrapper");

            // Name
            var name = doc.QuerySelector("body > div.main-wrapper > div:nth-child(3) > div > div > div:nth-child(2) > div > div > div > div > div > div > div > div > h1");

            // Price
            var price = doc.QuerySelector("body > div.main-wrapper > div:nth-child(3) > div > div > div:nth-child(2) > div > div > div > div > div > div > div> div:nth-child(4) > div:nth-child(1)");

            // 
            var shippingCost = doc.QuerySelector("body > div.main-wrapper > div:nth-child(3) > div > div > div:nth-child(2) > div > div > div > div > div > div > div > div > div > div > div:nth-child(1) > div:nth-child(3) > a");


            System.Console.WriteLine("> Original selector");
            System.Console.WriteLine("body > div.main-wrapper > div:nth-child(3) > div > div > div:nth-child(2) > div > div > div > div > div > div > div > div > h1");
            var path = name.GetSelector();


            Debugger.Break();
        }

        public static async void TwoAsync()
        {
            var doc = await WebLoader.LoadAsync("https://www.amazon.com/Waste-King-L-2600-Garbage-Disposal/dp/B0014X7B54/");

            var price = doc.QuerySelector("#priceblock_ourprice");

            var title = doc.QuerySelector("#productTitle");
            var away1 = doc.QuerySelector("#comparison_title0 > span");
            var away2 = doc.QuerySelector("#anonCarousel1 > ol > li:nth-child(1) > div > a > div:nth-child(2)");


            price.FindClosest(n =>
            {
                return n.Contains(title);
            }, out var node, out var depth);

            Debugger.Break();
        }

        public static async void ThreeAsync()
        {
            var doc = await WebLoader.LoadReadyAsync("https://bodyhouse.pl/product-pol-7854-Swanson-Ashwagandha-100caps.html");
            var bar = doc;

            Debugger.Break();
        }

        public static int StreamPos(this IElement element)
        {
            var inne = element.InnerHtml;
            var outt = element.OuterHtml;
            foreach (var item in element.Ancestors())
            {
                ;
            }
            return 0;
        }
    }
}
