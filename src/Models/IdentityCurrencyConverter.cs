namespace Diversifolio;

public sealed class IdentityCurrencyConverter : ICurrencyConverter
{
    public static IdentityCurrencyConverter Instance { get; } = new();

    public bool TryConvertFrom(CurrencyAmount source, out CurrencyAmount result)
    {
        result = source;
        return true;
    }
}
