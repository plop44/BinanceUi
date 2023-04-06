using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace BinanceUi.Services.OrderBooks;

public class OrderBookSnapshot
{
    [JsonPropertyName("lastUpdateId")]
    public long LastUpdateId { get; set; }

    [JsonPropertyName("bids"), JsonConverter(typeof(OrderBookEntryConverter))]
    public IEnumerable<OrderBookEntry> Bids { get; set; } = Enumerable.Empty<OrderBookEntry>();

    [JsonPropertyName("asks"), JsonConverter(typeof(OrderBookEntryConverter))]
    public IEnumerable<OrderBookEntry> Asks { get; set; } = Enumerable.Empty<OrderBookEntry>();
}