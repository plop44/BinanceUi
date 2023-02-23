using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using BinanceUi.Screens.Tickers;

namespace BinanceUi.Screens;

public class NewPageNavigator : IDisposable
{
    private readonly Subject<SymbolTickerViewModel> _symbolTickers = new();
    private readonly Func<SymbolTickerViewModelArguments, SymbolTickerViewModel> _symbolTickerViewModelFactory;

    public NewPageNavigator(Func<SymbolTickerViewModelArguments, SymbolTickerViewModel> symbolTickerViewModelFactory)
    {
        _symbolTickerViewModelFactory = symbolTickerViewModelFactory;
    }

    public void OpenPriceTracker(SymbolTickerViewModelArguments symbol)
    {
        _symbolTickers.OnNext(_symbolTickerViewModelFactory.Invoke(symbol));
    }

    public IObservable<SymbolTickerViewModel> AddSymbolTicker => _symbolTickers.AsObservable();

    public void Dispose()
    {
        _symbolTickers.OnCompleted();
        _symbolTickers.Dispose();
    }
}