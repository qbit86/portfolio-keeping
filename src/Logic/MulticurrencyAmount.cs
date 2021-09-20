using System;
using System.Collections.Generic;

namespace Diversifolio
{
    // TODO: Make readonly struct.
    public sealed class MulticurrencyAmount
    {
        private readonly Dictionary<string, CurrencyAmount> _currencyAmountByCurrency = new(StringComparer.Ordinal);

        public IReadOnlyDictionary<string, CurrencyAmount> CurrencyAmountByCurrency => _currencyAmountByCurrency;

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
    }
}
