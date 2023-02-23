using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BinanceUi.Screens.Tickers;
using BinanceUi.Screens.TradingPairs;
using BinanceUi.Various;

namespace BinanceUi.Screens;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly ObservableCollection<object> _items = new();
    private readonly Func<SymbolTickersViewModel> _symbolTickersViewModelFactory;
    private readonly Func<TradingInfoViewModel> _tradingInfoViewModelsFactory;

    private object? _selectedItem;

    public MainWindowViewModel(Func<TradingInfoViewModel> tradingInfoViewModelsFactory, NewPageNavigator newPageNavigator,
        Func<SymbolTickersViewModel> symbolTickersViewModelFactory)
    {
        _tradingInfoViewModelsFactory = tradingInfoViewModelsFactory;
        _symbolTickersViewModelFactory = symbolTickersViewModelFactory;
        AddTradingPairInfo = new DelegateCommand(OnAddTradingPairInfoExecute);

        newPageNavigator.AddSymbolTicker.Subscribe(OnAddSymbolTicker);
    }

    public object? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (value == _selectedItem)
                return;

            _selectedItem = value;
            OnPropertyChanged();
        }
    }

    public IEnumerable<object> Items => _items;
    public ICommand AddTradingPairInfo { get; }
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnAddSymbolTicker(SymbolTickerViewModel symbolTickerViewModel)
    {
        var symbolTickersViewModel = _items.OfType<SymbolTickersViewModel>().FirstOrDefault();

        if (symbolTickersViewModel != null)
        {
            symbolTickersViewModel.Add(symbolTickerViewModel);
            SelectedItem = symbolTickersViewModel;
            return;
        }

        symbolTickersViewModel = _symbolTickersViewModelFactory.Invoke();
        symbolTickersViewModel.Add(symbolTickerViewModel);

        _items.Add(symbolTickersViewModel);
    }

    private void OnAddTradingPairInfoExecute()
    {
        _items.Add(_tradingInfoViewModelsFactory.Invoke());
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}