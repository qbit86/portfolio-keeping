using System;

namespace Diversifolio
{
    public readonly struct CurrencyAmount : IEquatable<CurrencyAmount>
    {
        public CurrencyAmount(string currency, decimal amount)
        {
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            Amount = amount;
        }

        public string Currency { get; }
        public decimal Amount { get; }

        public bool Equals(CurrencyAmount other) => Currency == other.Currency && Amount == other.Amount;

        public override bool Equals(object? obj) => obj is CurrencyAmount other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Currency, Amount);

        public static bool operator ==(CurrencyAmount left, CurrencyAmount right) => left.Equals(right);

        public static bool operator !=(CurrencyAmount left, CurrencyAmount right) => !left.Equals(right);
    }
}
