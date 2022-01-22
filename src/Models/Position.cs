using System;
using System.Globalization;
using System.Text;

namespace Diversifolio;

public readonly struct Position : IEquatable<Position>, IFormattable
{
    public Position(string ticker, decimal balance)
    {
        Ticker = ticker ?? throw new ArgumentNullException(nameof(ticker));
        Balance = balance;
    }

    public string Ticker { get; }
    public decimal Balance { get; }

    public bool Equals(Position other) => Ticker == other.Ticker && Balance == other.Balance;

    public override bool Equals(object? obj) => obj is Position other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Ticker, Balance);

    public static bool operator ==(Position left, Position right) => left.Equals(right);

    public static bool operator !=(Position left, Position right) => !left.Equals(right);

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        StringBuilder sb = new(50);
        sb.Append(nameof(Position) + " {");
        sb.Append(" " + nameof(Ticker) + " = ");
        sb.Append(Ticker);
        sb.Append(", " + nameof(Balance) + " = ");
        FormattingHelpers.AppendDecimal(sb, Balance, format, formatProvider);
        sb.Append(" }");
        return sb.ToString();
    }

    public override string ToString() => ToString("G", CultureInfo.InvariantCulture);
}
