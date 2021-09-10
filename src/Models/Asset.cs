using System;

namespace Diversifolio
{
    public sealed record Asset(
        string Ticker, int Balance, string Board, CurrencyAmount Price, int DecimalCount, int LotSize)
    {
        public string Ticker { get; } = Ticker ?? throw new ArgumentNullException(nameof(Ticker));
        public string Board { get; } = Board ?? throw new ArgumentNullException(nameof(Board));
    }
}
