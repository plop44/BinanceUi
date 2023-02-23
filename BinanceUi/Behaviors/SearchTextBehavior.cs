using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Data;
using BinanceUi.Various;
using Microsoft.Xaml.Behaviors;

namespace BinanceUi.Behaviors;

public class SearchTextBehavior : Behavior<FrameworkElement>
{
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource), typeof(INotifyCollectionChanged), typeof(SearchTextBehavior), new PropertyMetadata(default(INotifyCollectionChanged)));

    public static readonly DependencyProperty FilteredItemsSourceProperty = DependencyProperty.Register(
        nameof(FilteredItemsSource), typeof(ICollectionView), typeof(SearchTextBehavior), new PropertyMetadata(default(ICollectionView)));

    public static readonly DependencyProperty SearchedTextProperty = DependencyProperty.Register(
        nameof(SearchedText), typeof(string), typeof(SearchTextBehavior), new PropertyMetadata(default(string)));

    private CompositeDisposable? _compositeDisposable;

    public INotifyCollectionChanged ItemsSource
    {
        get => (INotifyCollectionChanged)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public ICollectionView FilteredItemsSource
    {
        get => (ICollectionView)GetValue(FilteredItemsSourceProperty);
        set => SetValue(FilteredItemsSourceProperty, value);
    }

    public string SearchedText
    {
        get => (string)GetValue(SearchedTextProperty);
        set => SetValue(SearchedTextProperty, value);
    }


    protected override void OnAttached()
    {   
        base.OnAttached();

        var schedulerRepository = (SchedulerRepository)AssociatedObject.FindResource(SchedulerRepository.ResourceName);

        _compositeDisposable?.Dispose();

        _compositeDisposable = new CompositeDisposable();

        var subscription1 = this.ToObservable<INotifyCollectionChanged>(ItemsSourceProperty)
            .Where(t => t != null)
            .Subscribe(t =>
            {
                var filteredItemsSource = CollectionViewSource.GetDefaultView(t);
                filteredItemsSource.Filter = OnFilter;
                FilteredItemsSource = filteredItemsSource;
                filteredItemsSource.Refresh();
            });

        var subscription2 = this.ToObservable<string>(SearchedTextProperty)
            .Throttle(TimeSpan.FromMilliseconds(50))
            .ObserveOn(schedulerRepository.SynchronizationContextScheduler)
            .Subscribe(_ => { FilteredItemsSource?.Refresh(); });

        _compositeDisposable.Add(subscription1);
        _compositeDisposable.Add(subscription2);
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        _compositeDisposable?.Dispose();
    }

    private bool OnFilter(object obj)
    {
        if (obj is not ISearchable searchable) return true;

        var searchedText = SearchedText;

        if (string.IsNullOrEmpty(searchedText)) return true;

        return searchable.MatchesSearch(searchedText);
    }
}