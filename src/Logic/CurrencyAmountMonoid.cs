using System;
using System.Diagnostics.CodeAnalysis;

namespace Diversifolio;

public sealed class CurrencyAmountMonoid : IMonoid<CurrencyAmount>
{
    public static CurrencyAmountMonoid Instance { get; } = new();

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

    public static CurrencyAmount Multiply(decimal multiplier, CurrencyAmount value)
    {
        string currency = value.Currency;
        decimal amount = multiplier * value.Amount;
        return new(currency, amount);
    }

    public static decimal Divide(CurrencyAmount numerator, CurrencyAmount denominator)
    {
        string currency = numerator.Currency;
        if (currency != denominator.Currency)
            ThrowArgumentException(nameof(denominator));

        return numerator.Amount / denominator.Amount;
    }

    [DoesNotReturn]
    private static void ThrowArgumentException(string paramName) =>
        throw new ArgumentException("Currencies must be equal.", paramName);
}
