namespace Diversifolio
{
    public interface ICurrencyConverter
    {
        bool TryConvertFrom(CurrencyAmount source, out CurrencyAmount result);

        bool TryConvertTo(CurrencyAmount target, out CurrencyAmount result);
    }
}
