using System;

namespace Diversifolio.Moex;

public sealed record Bem(string Board, string Engine, string Market)
{
    public string Board { get; } = Board ?? throw new ArgumentNullException(nameof(Board));
    public string Engine { get; } = Engine ?? throw new ArgumentNullException(nameof(Engine));
    public string Market { get; } = Market ?? throw new ArgumentNullException(nameof(Market));
}
