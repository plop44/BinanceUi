namespace BinanceUi.Services;

public class SymbolItem
{
    public SymbolItem(string symbol, string baseAsset, string quoteAsset)
    {
        Symbol = symbol;
        BaseAsset = baseAsset;
        QuoteAsset = quoteAsset;
    }

    public string Symbol { get; }
    public string BaseAsset { get; }
    public string QuoteAsset { get; }
}