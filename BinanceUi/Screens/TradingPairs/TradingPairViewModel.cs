using System;
using System.ComponentModel;
using BinanceUi.Behaviors;
using BinanceUi.Services;

namespace BinanceUi.Screens.TradingPairs;

public class TradingPairViewModel : INotifyPropertyChanged // we need here to implement INotifyPropertyChanged to avoid a binding leak.
    , ISearchable
{
    private readonly SymbolItem _symbolItem;

    public TradingPairViewModel(SymbolItem symbolItem, decimal? lastPrice)
    {
        _symbolItem = symbolItem;
        LastPrice = lastPrice;
        BaseAsset = symbolItem.BaseAsset;
        QuoteAsset = symbolItem.QuoteAsset;
        Symbol = symbolItem.Symbol;
        DisplaySymbol = $"{BaseAsset}/{QuoteAsset}";
    }

    public string Symbol { get; }
    public string BaseAsset { get; }
    public string QuoteAsset { get; }
    public string DisplaySymbol { get; }
    public decimal? LastPrice { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool MatchesSearch(string searchedValue)
    {
        return Symbol.Contains(searchedValue, StringComparison.InvariantCultureIgnoreCase);
    }

    // method as we don't want to bind on it. It is used as 'DTO'.
    public SymbolItem GetSymbolItem() => _symbolItem;
}