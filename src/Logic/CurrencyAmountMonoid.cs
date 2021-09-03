using System;
using System.Diagnostics.CodeAnalysis;

namespace Diversifolio
{
    internal sealed class CurrencyAmountMonoid : IMonoid<CurrencyAmount>
    {
        internal static CurrencyAmountMonoid Instance { get; } = new();

        public CurrencyAmount Identity => CurrencyAmount.Empty;

        public CurrencyAmount Combine(CurrencyAmount left, CurrencyAmount right)
        {
            if (left.IsDefaultOrEmpty)
                return right;
            if (right.IsDefaultOrEmpty)
                return left;

            string currency = left.Currency;
            if (currency != right.Currency)
                ThrowArgumentException(nameof(right));

            decimal amount = left.Amount + right.Amount;
            return new(currency, amount);
        }

        [DoesNotReturn]
        private static void ThrowArgumentException(string paramName) =>
            throw new ArgumentException("Currencies must be equal.", paramName);
    }
}
