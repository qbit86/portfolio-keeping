using System;

namespace Diversifolio;

public interface ICurrencyConverter
{
    bool TryConvertFrom(CurrencyAmount source, out CurrencyAmount result);
}

public static class CurrencyConverterExtensions
{
    public static CurrencyAmount ConvertFrom<TCurrencyConverter>(
        this TCurrencyConverter converter, CurrencyAmount source)
        where TCurrencyConverter : ICurrencyConverter
    {
        if (converter is null)
            throw new ArgumentNullException(nameof(converter));

        return converter.TryConvertFrom(source, out CurrencyAmount result)
            ? result
            : throw new InvalidOperationException($"Cannot convert from {source.Currency}.");
    }
}
