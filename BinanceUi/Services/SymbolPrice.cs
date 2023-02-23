namespace BinanceUi.Services;

public class SymbolPrice
{
    public SymbolPrice(string symbol, string price) => (Symbol, Price) = (symbol, price);

    public string Symbol { get; }
    public string Price { get; }
}