namespace BinanceUi.Services;

public class SymbolItem
{
    public SymbolItem(string symbol, string baseAsset, string quoteAsset, string status)
    {
        Symbol = symbol;
        BaseAsset = baseAsset;
        QuoteAsset = quoteAsset;
        Status = status;
    }

    public string Symbol { get; }
    public string BaseAsset { get; }
    public string QuoteAsset { get; }
    public string Status { get; }
}

public static class SymbolItemExtensions
{
    public static bool IsTrading(this SymbolItem symbolItem) => symbolItem.Status == "TRADING";
    public static string GetDisplaySymbol(this SymbolItem symbolItem) => $"{symbolItem.BaseAsset}/{symbolItem.QuoteAsset}";
}