using System;
using System.Collections.Generic;

namespace Diversifolio
{
    public sealed class MulticurrencyAmount
    {
        private readonly Dictionary<string, CurrencyAmount> _currencyAmountByCurrency = new(StringComparer.Ordinal);

        public IReadOnlyDictionary<string, CurrencyAmount> CurrencyAmountByCurrency => _currencyAmountByCurrency;
    }
}
