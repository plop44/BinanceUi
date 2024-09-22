using System.Text.Json.Serialization;

namespace BinanceUi.Services;

public record TickerData(
    [property: JsonPropertyName("s")] string Symbol,
    [property: JsonPropertyName("E")] long EventTime,
    [property: JsonPropertyName("c")] string LastPrice,
    [property: JsonPropertyName("Q")] string LastQuantity
);

public record BinanceWebSocketResponse(
    [property: JsonPropertyName("result")] object Result,
    [property: JsonPropertyName("id")] int Id);

public record SubscriptionMessage(
    [property: JsonPropertyName("method")] string Method,
    [property: JsonPropertyName("params")] string[] Params,
    // we do not use Id at this point
    [property: JsonPropertyName("id")] int Id
)
{
    public static SubscriptionMessage GetTickerSubscribe(string symbol) => new SubscriptionMessage("SUBSCRIBE", new[] { $"{symbol.ToLowerInvariant()}@ticker" }, 0);
    public static SubscriptionMessage GetTickerUnsubscribe(string symbol) => new SubscriptionMessage("UNSUBSCRIBE", new[] { $"{symbol.ToLowerInvariant()}@ticker" }, 0);
    public bool IsUnsubscribe() => Method == "UNSUBSCRIBE";
    
    // will only work for single subscriptions
    public string GetKey() => Params[0];
}