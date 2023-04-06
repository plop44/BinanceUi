using System.ComponentModel;

namespace BinanceUi.Screens.OrderBooks;

public class OrderBookEntryViewModel : INotifyPropertyChanged
{
    public decimal Price { get; }
    public decimal Quantity { get; }

    public class Parameters
    {
        public decimal Price { get; init; }
        public decimal Quantity { get; init; }
    }

    public OrderBookEntryViewModel(Parameters parameters)
    {
        Price = parameters.Price;
        Quantity = parameters.Quantity;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}