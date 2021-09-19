using System;
using Diversifolio.Moex;

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

        public static RubCurrencyConverter Create(SeltSecurity usd)
        {
            if (usd is null)
                throw new ArgumentNullException(nameof(usd));
            if (usd.CurrencyId != "RUB")
                throw new ArgumentException($"The {nameof(usd.CurrencyId)} value must be equal to RUB.", nameof(usd));
            if (usd.FaceUnit != "USD")
                throw new ArgumentException($"The {nameof(usd.FaceUnit)} value must be equal to USD.", nameof(usd));

            return new(usd.PrevPrice);
        }

        public CurrencyAmount ConvertFrom(CurrencyAmount source) => TryConvertFrom(source, out CurrencyAmount result)
            ? result
            : throw new InvalidOperationException($"Cannot convert from {source.Currency}.");
    }
}
