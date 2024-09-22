using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BinanceUi.Screens.OrderBooks;

// source https://stackoverflow.com/questions/9895394/how-to-insert-an-item-at-the-beginning-of-an-observablecollection
// collection 
public class ObservableStack<T> : Stack<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
    public ObservableStack()
    {
    }

    public ObservableStack(IEnumerable<T> collection)
    {
        foreach (var item in collection)
            base.Push(item);
    }


    public event NotifyCollectionChangedEventHandler? CollectionChanged;


    public event PropertyChangedEventHandler? PropertyChanged;


    public new void Clear()
    {
        base.Clear();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public new T Pop()
    {
        var item = base.Pop();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, 0));
        return item;
    }

    public new void Push(T item)
    {
        base.Push(item);
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, 0));
    }


    protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        RaiseCollectionChanged(e);
    }

    protected void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        RaisePropertyChanged(e);
    }

    private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
    }

    private void RaisePropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }
}