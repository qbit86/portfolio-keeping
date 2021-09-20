using System;
using Diversifolio.Moex;
using static Diversifolio.TryHelpers;

namespace Diversifolio
{
    public readonly struct RubCurrencyConverter : ICurrencyConverter, IEquatable<RubCurrencyConverter>
    {
        private readonly decimal _rubPerUsd;

        public RubCurrencyConverter(decimal rubPerUsd) => _rubPerUsd = rubPerUsd;

        public bool TryConvertFrom(CurrencyAmount source, out CurrencyAmount result) =>
            source.Currency switch
            {
                "SUR" => Success(source, out result),
                "RUB" => Success(new("SUR", source.Amount), out result),
                "USD" => Success(new("SUR", source.Amount * _rubPerUsd), out result),
                _ => Failure(out result)
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

        public bool Equals(RubCurrencyConverter other) => _rubPerUsd == other._rubPerUsd;

        public override bool Equals(object? obj) => obj is RubCurrencyConverter other && Equals(other);

        public override int GetHashCode() => _rubPerUsd.GetHashCode();

        public static bool operator ==(RubCurrencyConverter left, RubCurrencyConverter right) => left.Equals(right);

        public static bool operator !=(RubCurrencyConverter left, RubCurrencyConverter right) => !left.Equals(right);
    }
}
