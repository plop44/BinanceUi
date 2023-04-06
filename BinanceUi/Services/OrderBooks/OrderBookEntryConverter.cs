using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinanceUi.Services.OrderBooks;

public class OrderBookEntryConverter : JsonConverter<IEnumerable<OrderBookEntry>>
{
    public override List<OrderBookEntry> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);

        var entries = new List<OrderBookEntry>();

        for (int i = 0; i < doc.RootElement.GetArrayLength(); i++)
        {
            var price = decimal.Parse(doc.RootElement[i][0].GetString());
            var quantity = decimal.Parse(doc.RootElement[i][1].GetString());

            entries.Add(new OrderBookEntry { Price = price, Quantity = quantity });
        }

        return entries;
    }

    public override void Write(Utf8JsonWriter writer, IEnumerable<OrderBookEntry> value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
    }
}