namespace BinanceUi.Services;

public static class TickerDataExtensions
{
    public static decimal? LastPriceDecimal(this TickerData tickerData)
    {
        var value = tickerData.LastPrice;
        return string.IsNullOrEmpty(value) ? null : decimal.Parse(value);
    }

    public static decimal? LastQuantityDecimal(this TickerData tickerData)
    {
        var value = tickerData.LastQuantity;
        return string.IsNullOrEmpty(value) ? null : decimal.Parse(value);
    }
}