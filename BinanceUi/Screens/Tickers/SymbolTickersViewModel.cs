using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BinanceUi.Screens.Tickers;

public class SymbolTickersViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly ObservableCollection<SymbolTickerViewModel> _items = new();

    public IEnumerable<SymbolTickerViewModel> Items => _items;

    public void Dispose()
    {
        foreach (var symbolTickerViewModel in _items) symbolTickerViewModel.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Add(SymbolTickerViewModel symbolTickerViewModel)
    {
        _items.Add(symbolTickerViewModel);
    }
}