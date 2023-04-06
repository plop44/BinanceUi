using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BinanceUi.Services.OrderBooks;

namespace BinanceUi.Services;

public class BinanceService
{
    private const string ApiBaseUrl = "https://api.binance.com/api/v3";
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNameCaseInsensitive = false
    };

    public BinanceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ImmutableList<SymbolItem>> GetAllTradingPairs()
    {
        var response = await _httpClient.GetAsync($"{ApiBaseUrl}/exchangeInfo").ConfigureAwait(false);

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ExchangeInfoResponse>(content, options)?.Symbols.Where(t=>t.IsTrading()).ToImmutableList()
               ?? ImmutableList.Create<SymbolItem>();
    }

    public async Task<ImmutableDictionary<string, decimal>> GetAllLatestPrices()
    {
        var response = await _httpClient.GetAsync($"{ApiBaseUrl}/ticker/price").ConfigureAwait(false);

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<SymbolPrice[]>(content, options)
                   ?.ToImmutableDictionary(t => t.Symbol, t => decimal.Parse(t.Price))
               ?? ImmutableDictionary.Create<string, decimal>();
    }


    public IObservable<TickerData> GetSymbolTicker(string symbol)
    {
        var url = new Uri($"wss://stream.binance.com:9443/ws/{symbol.ToLowerInvariant()}@ticker");
        return GetWebsocketObservable<TickerData>(url);
    }

    private IObservable<T> GetWebsocketObservable<T>(Uri url)
    {
        return Observable.Create<T>(async (observer, cancellationToken) =>
        {
            var buffer = new byte[1<<12];

            try
            {
                using var clientWebSocket = new ClientWebSocket();
                await clientWebSocket.ConnectAsync(url, cancellationToken);

                while (clientWebSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                {
                    var result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Text:
                        {
                            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            var item = JsonSerializer.Deserialize<T>(message, _serializerOptions);
                            observer.OnNext(item!);
                            break;
                        }
                        case WebSocketMessageType.Close:
                            await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", cancellationToken);
                            observer.OnCompleted();
                            break;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // RX subscription has been disposed
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
            }
        });
    }

    public async Task<OrderBookSnapshot> GetOrderBookSnapshot(string symbol, int limit = 100)
    {
        var restUrl = $"https://api.binance.com/api/v3/depth?symbol={symbol.ToUpperInvariant()}&limit={limit}";
        var restResponse = await _httpClient.GetAsync(restUrl);
        restResponse.EnsureSuccessStatusCode();
        var contentString = await restResponse.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<OrderBookSnapshot>(contentString)!;
    }

    public IObservable<OrderBookSnapshot> GetOrderBookUpdates(string symbol, OrderBookDepth orderBookDepth = OrderBookDepth.D20, OrderBookRefreshSpeed speed = OrderBookRefreshSpeed.S100)
    {
        var orderBookDepthString = orderBookDepth switch
        {
            OrderBookDepth.D5 => 5,
            OrderBookDepth.D10 => 10,
            OrderBookDepth.D20 => 20,
            _ => throw new ArgumentOutOfRangeException(nameof(orderBookDepth), orderBookDepth, "Please add value")
        };

        var speedString = speed switch
        {
            OrderBookRefreshSpeed.S100 => 100,
            OrderBookRefreshSpeed.S1000 => 1000,
            _ => throw new ArgumentOutOfRangeException(nameof(speed), speed, "Please add value")
        };

        var url = new Uri($"wss://stream.binance.com:9443/ws/{symbol.ToLowerInvariant()}@depth{orderBookDepthString}@{speedString}ms");
        return GetWebsocketObservable<OrderBookSnapshot>(url);
    }
}
public enum OrderBookDepth {D5, D10, D20}
public enum OrderBookRefreshSpeed {S100,S1000}