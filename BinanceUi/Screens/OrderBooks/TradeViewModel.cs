using System;
using System.ComponentModel;
using BinanceUi.Services;

namespace BinanceUi.Screens.OrderBooks;

public class TradeViewModel : INotifyPropertyChanged
{
    public TradeViewModel(AggTrade trade)
    {
        Price = trade.PriceTyped;
        Quantity = trade.QuantityTyped;
        TradeTime = trade.TradeTimeTyped;
    }

    public DateTimeOffset TradeTime { get; }
    public decimal Price { get; }
    public decimal Quantity { get; }

    public event PropertyChangedEventHandler? PropertyChanged;
}