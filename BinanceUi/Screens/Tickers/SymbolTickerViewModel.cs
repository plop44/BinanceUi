using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using BinanceUi.Services;
using BinanceUi.Various;

namespace BinanceUi.Screens.Tickers;

public class SymbolTickerViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly IDisposable _subscription;

    private bool? _isUp;
    private decimal? _lastPrice;
    private decimal? _quantity;

    public SymbolTickerViewModel(SymbolItem arguments, BinanceService binanceService, SchedulerRepository schedulerRepository)
    {
        Symbol = arguments.Symbol;
        DisplaySymbol = arguments.GetDisplaySymbol();

        _subscription = binanceService
            .GetSymbolTicker(arguments.Symbol)
            .ObserveOn(schedulerRepository.SynchronizationContextScheduler)
            .Subscribe(t =>
            {
                var newPrice = t.LastPriceDecimal();
                IsUp = GetIsUp(_lastPrice, newPrice);
                LastPrice = newPrice;
                Quantity = t.LastQuantityDecimal();
            });
    }

    public bool? IsUp
    {
        get => _isUp;
        private set
        {
            if (value == _isUp)
                return;

            _isUp = value;
            OnPropertyChanged();
        }
    }

    public string Symbol { get; }
    public string DisplaySymbol { get; }

    public decimal? LastPrice
    {
        get => _lastPrice;
        private set
        {
            if (value == _lastPrice)
                return;

            _lastPrice = value;
            OnPropertyChanged();
        }
    }

    public decimal? Quantity
    {
        get => _quantity;
        private set
        {
            if (value == _quantity)
                return;

            _quantity = value;
            OnPropertyChanged();
        }
    }

    public void Dispose()
    {
        _subscription.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool? GetIsUp(decimal? previousPrice, decimal? newPrice)
    {
        if (!previousPrice.HasValue || !newPrice.HasValue) return null;
        // in case price is same as previous, we keep the same IsUp as before. 
        // it is because quantity (or another property) can change, without price to change.
        if (newPrice.Value == previousPrice.Value) return _isUp;
        return newPrice.Value > previousPrice.Value;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}