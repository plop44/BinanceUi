using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BinanceUi.Services.OrderBooks;

namespace BinanceUi.Services;

public class BinanceService
{
    // https://binance.github.io/binance-api-swagger/
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
        return JsonSerializer.Deserialize<ExchangeInfoResponse>(content, options)?.Symbols.Where(t => t.IsTrading()).ToImmutableList()
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

    public async Task<OrderBookSnapshot> GetOrderBookSnapshot(string symbol, int limit = 100)
    {
        var restUrl = $"https://api.binance.com/api/v3/depth?symbol={symbol.ToUpperInvariant()}&limit={limit}";
        var restResponse = await _httpClient.GetAsync(restUrl);
        restResponse.EnsureSuccessStatusCode();
        var contentString = await restResponse.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<OrderBookSnapshot>(contentString)!;
    }
}