using System;
using System.Globalization;
using System.Text;

namespace Diversifolio
{
    public readonly struct CurrencyAmount : IEquatable<CurrencyAmount>, IFormattable
    {
        public CurrencyAmount(string currency, decimal amount)
        {
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            Amount = amount;
        }

        public static CurrencyAmount Empty { get; } = new(string.Empty, 0m);

        public string Currency { get; }
        public decimal Amount { get; }
        public bool IsDefaultOrEmpty => string.IsNullOrEmpty(Currency) && Amount == 0m;

        public bool Equals(CurrencyAmount other) => Currency == other.Currency && Amount == other.Amount;

        public override bool Equals(object? obj) => obj is CurrencyAmount other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Currency, Amount);

        public static bool operator ==(CurrencyAmount left, CurrencyAmount right) => left.Equals(right);

        public static bool operator !=(CurrencyAmount left, CurrencyAmount right) => !left.Equals(right);

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            StringBuilder sb = new(50);
            sb.Append(nameof(CurrencyAmount) + " {");
            sb.Append(" " + nameof(Currency) + " = ");
            sb.Append(Currency);
            sb.Append(", " + nameof(Amount) + " = ");
            FormattingHelpers.AppendDecimal(sb, Amount, format, formatProvider);
            sb.Append(" }");
            return sb.ToString();
        }

        public override string ToString() => ToString("G", CultureInfo.InvariantCulture);
    }
}
