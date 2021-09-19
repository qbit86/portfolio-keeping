using System;

namespace Diversifolio
{
    public sealed class RubCurrencyConverter : ICurrencyConverter
    {
        private readonly decimal _rubPerUsd;

        public RubCurrencyConverter(decimal rubPerUsd) => _rubPerUsd = rubPerUsd;

        public bool TryConvertFrom(CurrencyAmount source, out CurrencyAmount result) =>
            source.Currency switch
            {
                "SUR" => TryHelpers.Success(source, out result),
                "RUB" => TryHelpers.Success(new("SUR", source.Amount), out result),
                "USD" => TryHelpers.Success(new("SUR", source.Amount * _rubPerUsd), out result),
                _ => TryHelpers.Failure(out result)
            };

        public CurrencyAmount ConvertFrom(CurrencyAmount source) => TryConvertFrom(source, out CurrencyAmount result)
            ? result
            : throw new InvalidOperationException($"Cannot convert from {source.Currency}.");
    }
}
