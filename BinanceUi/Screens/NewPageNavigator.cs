using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using BinanceUi.Screens.OrderBooks;
using BinanceUi.Screens.Tickers;
using BinanceUi.Services;

namespace BinanceUi.Screens;

public class NewPageNavigator : IDisposable
{
    private readonly Subject<SymbolTickerViewModel> _symbolTickers = new();
    private readonly Subject<OrderBookViewModel> _orderBooks = new();
    private readonly Func<SymbolItem, SymbolTickerViewModel> _symbolTickerViewModelFactory;
    private readonly Func<SymbolItem, OrderBookViewModel> _orderBookViewModelFactory;

    public NewPageNavigator(Func<SymbolItem, SymbolTickerViewModel> symbolTickerViewModelFactory, Func<SymbolItem, OrderBookViewModel> orderBookViewModelFactory)
    {
        _symbolTickerViewModelFactory = symbolTickerViewModelFactory;
        _orderBookViewModelFactory = orderBookViewModelFactory;
    }

    public void OpenPriceTracker(SymbolItem symbol)
    {
        _symbolTickers.OnNext(_symbolTickerViewModelFactory.Invoke(symbol));
    }

    public IObservable<SymbolTickerViewModel> AddSymbolTicker => _symbolTickers.AsObservable();
    public IObservable<OrderBookViewModel> AddOrderBook => _orderBooks.AsObservable();

    public void Dispose()
    {
        _symbolTickers.OnCompleted();
        _symbolTickers.Dispose();

        _orderBooks.OnCompleted();
        _orderBooks.Dispose();
    }

    public void OpenOrderBook(SymbolItem symbolItem)
    {
        _orderBooks.OnNext(_orderBookViewModelFactory.Invoke(symbolItem));
    }
}