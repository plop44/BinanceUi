using System;
using System.Collections.Immutable;
using System.Net.Http;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
        return JsonSerializer.Deserialize<ExchangeInfoResponse>(content, options)?.Symbols.ToImmutableList()
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

        return Observable.Create<TickerData>(async (observer, cancellationToken) =>
        {
            var buffer = new byte[1024];

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
                            var priceData = JsonSerializer.Deserialize<TickerData>(message, _serializerOptions);
                            observer.OnNext(priceData!);
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
}