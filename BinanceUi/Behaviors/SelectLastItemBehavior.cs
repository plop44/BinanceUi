using System;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Windows.Controls;
using BinanceUi.Various;
using Microsoft.Xaml.Behaviors;

namespace BinanceUi.Behaviors;

public class SelectLastItemBehavior : Behavior<TabControl>
{
    private IDisposable? _dispose;

    protected override void OnAttached()
    {
        base.OnAttached();

        var schedulerRepository = (SchedulerRepository)AssociatedObject.FindResource(SchedulerRepository.ResourceName);

        var addedItemObservable = AssociatedObject.ToObservable<INotifyCollectionChanged>(ItemsControl.ItemsSourceProperty)
            .Select(t => t?.ToObservable() ?? Observable.Empty<NotifyCollectionChangedEventArgs>())
            .Switch()
            // we only support single add atm
            .Where(t => t.Action == NotifyCollectionChangedAction.Add)
            .Select(t => t?.NewItems?[^1])
            .Where(t => t != null);

        _dispose?.Dispose();

        _dispose = AssociatedObject.LoadedAsObservable()
            .CombineLatest(addedItemObservable)
            .Where(t => t.First)
            .ObserveOn(schedulerRepository.ImmediateOrDispatcherScheduler)
            .Subscribe(t =>
            {
                AssociatedObject.SelectedItem = t.Second;
            });
    }

    protected override void OnDetaching()
    {
        _dispose?.Dispose();
    }
}