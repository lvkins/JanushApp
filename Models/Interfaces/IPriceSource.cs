namespace PromoSeeker
{
    public interface IPriceSource
    {
        string XPath { get; set; }

        decimal Price { get; set; }
    }
}
