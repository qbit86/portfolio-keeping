using System;
using System.Collections.Generic;

namespace Diversifolio;

public readonly struct MulticurrencyAmount : IEquatable<MulticurrencyAmount>
{
    private readonly Dictionary<string, CurrencyAmount> _currencyAmountByCurrency;

    private MulticurrencyAmount(Dictionary<string, CurrencyAmount> currencyAmountByCurrency) =>
        _currencyAmountByCurrency = currencyAmountByCurrency;

    public IReadOnlyDictionary<string, CurrencyAmount> CurrencyAmountByCurrency => _currencyAmountByCurrency;

    public static MulticurrencyAmount Create() => new(new(StringComparer.Ordinal));

    public void Add(CurrencyAmount currencyAmount)
    {
        if (currencyAmount.IsDefaultOrEmpty)
            return;

        string key = currencyAmount.Currency;
        CurrencyAmount value = _currencyAmountByCurrency.TryGetValue(key, out CurrencyAmount existingValue)
            ? CurrencyAmountMonoid.Instance.Combine(existingValue, currencyAmount)
            : currencyAmount;
        _currencyAmountByCurrency[key] = value;
    }

    public bool Equals(MulticurrencyAmount other) =>
        _currencyAmountByCurrency.Equals(other._currencyAmountByCurrency);

    public override bool Equals(object? obj) => obj is MulticurrencyAmount other && Equals(other);

    public override int GetHashCode() => _currencyAmountByCurrency.GetHashCode();

    public static bool operator ==(MulticurrencyAmount left, MulticurrencyAmount right) => left.Equals(right);

    public static bool operator !=(MulticurrencyAmount left, MulticurrencyAmount right) => !left.Equals(right);
}
