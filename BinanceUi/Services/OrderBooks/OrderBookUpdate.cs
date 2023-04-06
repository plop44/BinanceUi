using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace BinanceUi.Services.OrderBooks;

public class OrderBookUpdate
{
    [JsonPropertyName("s")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("U")]
    public long? FirstUpdateId { get; set; }

    [JsonPropertyName("u")]
    public long LastUpdateId { get; set; }

    [JsonPropertyName("b"), JsonConverter(typeof(OrderBookEntryConverter))]
    public IEnumerable<OrderBookEntry> Bids { get; set; } = Array.Empty<OrderBookEntry>();

    [JsonPropertyName("a"), JsonConverter(typeof(OrderBookEntryConverter))]
    public IEnumerable<OrderBookEntry> Asks { get; set; } = Array.Empty<OrderBookEntry>();
}