using System;
using System.Globalization;
using System.Text;

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

        public override string ToString()
        {
            StringBuilder sb = new(50);
            sb.Append(nameof(Position) + " { ");
            sb.Append(nameof(Ticker) + " = ");
            sb.Append(Ticker);
            sb.Append(", " + nameof(Balance) + " = ");
            // src/libraries/System.Private.CoreLib/src/System/Number.NumberBuffer.cs
            const int decimalNumberBufferLength = 29 + 1 + 1;
            const string balanceFormat = "G";
            Span<char> buffer = stackalloc char[decimalNumberBufferLength];
            if (Balance.TryFormat(buffer, out int charsWritten, balanceFormat, CultureInfo.InvariantCulture))
                sb.Append(buffer[..charsWritten]);
            else
                sb.Append(Balance.ToString(balanceFormat, CultureInfo.InvariantCulture));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}
