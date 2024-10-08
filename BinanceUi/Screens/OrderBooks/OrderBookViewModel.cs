﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using BinanceUi.Screens.Tickers;
using BinanceUi.Services;
using BinanceUi.Various;

namespace BinanceUi.Screens.OrderBooks;

public class OrderBookViewModel : IDisposable, INotifyPropertyChanged
{
    private readonly ObservableRangeCollection<OrderBookEntryViewModel> _asks = new();
    private readonly ObservableRangeCollection<OrderBookEntryViewModel> _bids = new();
    private readonly ObservableStack<TradeViewModel> _trades = new();
    private const OrderBookDepth OrderBookDepth = Services.OrderBookDepth.D20;
    private const int OrderBookDepthNumber = 20;
    private readonly CompositeDisposable _compositeDisposable = new();

    public OrderBookViewModel(SymbolItem symbolItem, BinanceService binanceService, SchedulerRepository schedulerRepository,
        Func<OrderBookEntryViewModel.Parameters, OrderBookEntryViewModel> factory, SymbolTickerViewModel symbolTickerViewModel,
        BinanceWebsocketService binanceWebsocketService)
    {
        DisplaySymbol = symbolItem.GetDisplaySymbol();
        SymbolTickerViewModel = symbolTickerViewModel;

        binanceService.GetOrderBookSnapshot(symbolItem.Symbol, OrderBookDepthNumber).ToObservable()
            .Concat(binanceWebsocketService.GetOrderBookUpdates(symbolItem.Symbol, OrderBookDepth))
            .ObserveOn(schedulerRepository.SynchronizationContextScheduler)
            .Subscribe(t =>
            {
                _bids.ReplaceRange(t.Bids.Select(b => factory.Invoke(new OrderBookEntryViewModel.Parameters { Price = b.Price, Quantity = b.Quantity })));
                _asks.ReplaceRange(t.Asks.Reverse().Select(ask => factory.Invoke(new OrderBookEntryViewModel.Parameters { Price = ask.Price, Quantity = ask.Quantity })));
            })
            .DisposeWith(_compositeDisposable);

        binanceWebsocketService
            .GetAggTrades(symbolItem.Symbol)
            .Select(t=>new TradeViewModel(t))
            .ObserveOn(schedulerRepository.SynchronizationContextScheduler)
            .Subscribe(t =>
            {
                _trades.Push(t);
            })
            .DisposeWith(_compositeDisposable);
    }

    public string DisplaySymbol { get; }

    public IEnumerable<OrderBookEntryViewModel> Bids => _bids;
    public IEnumerable<OrderBookEntryViewModel> Asks => _asks;
    public IEnumerable<TradeViewModel> Trades => _trades;

    public SymbolTickerViewModel SymbolTickerViewModel { get; }

    public void Dispose()
    {
        _compositeDisposable.Dispose();
        SymbolTickerViewModel.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}