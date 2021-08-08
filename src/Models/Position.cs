using System;

namespace Diversifolio
{
    public readonly struct Position : IEquatable<Position>
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
    }
}
