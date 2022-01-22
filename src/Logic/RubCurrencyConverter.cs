using System;
using Diversifolio.Moex;
using static Diversifolio.TryHelpers;

namespace Diversifolio;

public readonly struct RubCurrencyConverter : ICurrencyConverter, IEquatable<RubCurrencyConverter>
{
    private readonly decimal _rubPerUsd;

    public RubCurrencyConverter(decimal rubPerUsd) => _rubPerUsd = rubPerUsd;

    public bool TryConvertFrom(CurrencyAmount source, out CurrencyAmount result) =>
        source switch
        {
            ("SUR", _) => Success(source, out result),
            ("RUB", var amount) => Success(new("SUR", amount), out result),
            ("USD", var amount) => Success(new("SUR", amount * _rubPerUsd), out result),
            _ when source == CurrencyAmount.Empty => Success(new("SUR", 0m), out result),
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
