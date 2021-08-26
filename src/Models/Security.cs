using System;

namespace Diversifolio
{
    public sealed record Security(
        string SecId, string BoardId, string CurrencyId, int LotSize, decimal FaceValue, int Decimals,
        decimal PrevAdmittedQuote)
    {
        public string SecId { get; } = SecId ?? throw new ArgumentNullException(nameof(SecId));
        public string BoardId { get; } = BoardId ?? throw new ArgumentNullException(nameof(BoardId));
        public string CurrencyId { get; } = CurrencyId ?? throw new ArgumentNullException(nameof(CurrencyId));
    }
}
