using System;

namespace Diversifolio.Moex;

public sealed record SeltSecurity(string SecId, string BoardId, string ShortName, string CurrencyId, int Decimals,
    decimal FaceValue, string FaceUnit, decimal PrevPrice) : Security(SecId)
{
    public string BoardId { get; } = BoardId ?? throw new ArgumentNullException(nameof(BoardId));
    public string ShortName { get; } = ShortName ?? throw new ArgumentNullException(nameof(ShortName));
    public string CurrencyId { get; } = CurrencyId ?? throw new ArgumentNullException(nameof(CurrencyId));
}
