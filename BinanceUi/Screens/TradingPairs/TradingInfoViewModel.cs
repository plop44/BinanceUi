using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BinanceUi.Services;
using BinanceUi.Various;

namespace BinanceUi.Screens.TradingPairs;

public class TradingInfoViewModel : INotifyPropertyChanged
{
    private readonly BinanceService _binanceService;

    private readonly ObservableRangeCollection<TradingPairViewModel> _items = new();
    private readonly NewPageNavigator _newPageNavigator;

    public TradingInfoViewModel(BinanceService binanceService, NewPageNavigator newPageNavigator)
    {
        _binanceService = binanceService;
        _newPageNavigator = newPageNavigator;
        OpenPriceTicker = new DelegateCommand<IList>(OnOpenPriceTickerExecute, CanOpenPriceTickerExecute);
        OpenOrderBook = new DelegateCommand<IList>(CanOpenOrderBookExecute, CanOpenPriceTickerExecute);
        Initialized = Init();
    }

    public IEnumerable<TradingPairViewModel> Items => _items;

    public Task Initialized { get; }
    public ICommand OpenPriceTicker { get; }
    public ICommand OpenOrderBook { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool CanOpenPriceTickerExecute(IList? tradingPairViewModels)
    {
        return tradingPairViewModels?.Count > 0;
    }

    private void OnOpenPriceTickerExecute(IList? tradingPairViewModels)
    {
        var pairViewModels = tradingPairViewModels?.OfType<TradingPairViewModel>();

        if(pairViewModels == null)
            return;

        foreach (var tradingPairViewModel in pairViewModels)
        {
            _newPageNavigator.OpenPriceTracker(tradingPairViewModel.GetSymbolItem());
        }
    }

    private void CanOpenOrderBookExecute(IList? tradingPairViewModels)
    {
        var pairViewModels = tradingPairViewModels?.OfType<TradingPairViewModel>();

        if (pairViewModels == null)
            return;

        foreach (var tradingPairViewModel in pairViewModels)
        {
            _newPageNavigator.OpenOrderBook(tradingPairViewModel.GetSymbolItem());
        }
    }

    private async Task Init()
    {
        var tradingPairViewModels = await GetTradingPairViewModel();
        _items.ReplaceRange(tradingPairViewModels);
    }

    private async Task<ImmutableArray<TradingPairViewModel>> GetTradingPairViewModel()
    {
        var tradingPairs = _binanceService.GetAllTradingPairs();
        var latestPrice = _binanceService.GetAllLatestPrices();

        await Task.WhenAll(tradingPairs, latestPrice).ConfigureAwait(false);

        return tradingPairs.Result.Select(t => new TradingPairViewModel(t, latestPrice.Result.TryGetValue(t.Symbol, out var price) ? price : null)).ToImmutableArray();
    }
}