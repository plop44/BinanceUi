using System.Text.Json.Serialization;

namespace BinanceUi.Services;

public class TickerData
{
    public TickerData(string symbol, long eventTime, string lastPrice, string lastQuantity)
    {
        Symbol = symbol;
        EventTime = eventTime;
        LastPrice = lastPrice;
        LastQuantity = lastQuantity;
    }

    [JsonPropertyName("s")]
    public string Symbol { get; set; }

    [JsonPropertyName("E")]
    public long EventTime { get; set; }

    [JsonPropertyName("c")]
    public string LastPrice { get; set; }

    [JsonPropertyName("Q")]
    public string LastQuantity { get; set; }
}