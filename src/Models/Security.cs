using System;

namespace Diversifolio
{
    public sealed record Security(
        string SecId, string BoardId, string CurrencyId, int LotSize, decimal FaceValue, int Decimals,
        decimal PrevAdmittedQuote)
    {
        public string SecId { get; init; } = SecId ?? throw new ArgumentNullException(nameof(SecId));
        public string BoardId { get; init; } = BoardId ?? throw new ArgumentNullException(nameof(BoardId));
        public string CurrencyId { get; init; } = CurrencyId ?? throw new ArgumentNullException(nameof(CurrencyId));
    }
}
