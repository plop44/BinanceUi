namespace BinanceUi.Screens.Tickers;

public class SymbolTickerViewModelArguments
{
    public SymbolTickerViewModelArguments(string symbol, string displaySymbol)
    {
        Symbol = symbol;
        DisplaySymbol = displaySymbol;
    }

    public string Symbol { get; }
    public string DisplaySymbol { get; }
}