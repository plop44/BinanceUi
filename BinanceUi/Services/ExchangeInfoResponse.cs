using System.Collections.Generic;

namespace BinanceUi.Services;

public class ExchangeInfoResponse
{
    public ExchangeInfoResponse(List<SymbolItem> symbols) => Symbols = symbols;

    public List<SymbolItem> Symbols { get; set; }
}