
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser.Tokens;
using AngleSharp.XPath;
using System.IO;

namespace PromoSeeker.Core
{
    public static class AngleSharpTest
    {
        public static async void OneAsync()
        {
            var address = "https://allegro.pl/oferta/nawiewnik-wrebowy-2-szt-kpl-zyj-zdrowo-bez-smogu-7705477010";
            var document = await WebLoader.LoadAsync(address);
            var priceCssSelector = "body > div.main-wrapper > div > nav > div._fee54_1lyr3._fee54_12CxQ > div > div:nth-child(1) > div";
            var priceXPathQuerySelector = "*[xpath>'//body/div/div/div/div/div/div/div/div/div/div/div/div[2]/div[5]/div']";
            var priceXPathQuery = "//body/div/div/div/div/div/div/div/div/div/div/div/div[2]/div[5]/div";

            var priceCss = document.QuerySelector(priceCssSelector);
            //var titles = cells.Select(m => m.TextContent);
        }
    }
}
