using System;

namespace Diversifolio;

public sealed record Asset(string Ticker, AssetClass AssetClass, string Board, int Balance,
    CurrencyAmount Price, CurrencyAmount Value, CurrencyAmount OriginalPrice, CurrencyAmount OriginalValue,
    int DecimalCount, int LotSize)
{
    public string Ticker { get; } = Ticker ?? throw new ArgumentNullException(nameof(Ticker));
    public string Board { get; } = Board ?? throw new ArgumentNullException(nameof(Board));
}
