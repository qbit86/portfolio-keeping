using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Diversifolio
{
    public static class ProportionFormatter
    {
        public static ProportionFormatter<TCurrencyConverter> Create<TCurrencyConverter>(
            TCurrencyConverter currencyConverter, CurrencyAmount total, IReadOnlyList<string> currencies,
            StringBuilder? stringBuilder = null)
            where TCurrencyConverter : ICurrencyConverter =>
            new(currencyConverter, total, currencies, stringBuilder);
    }

    public sealed class ProportionFormatter<TCurrencyConverter>
        where TCurrencyConverter : ICurrencyConverter
    {
        private const string Separator = " | ";

        private readonly IReadOnlyList<string> _currencies;
        private readonly TCurrencyConverter _currencyConverter;
        private readonly StringBuilder _stringBuilder;
        private readonly CurrencyAmount _total;

        public ProportionFormatter(TCurrencyConverter currencyConverter, CurrencyAmount total,
            IReadOnlyList<string> currencies, StringBuilder? stringBuilder = null)
        {
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            _total = total;
            _currencies = currencies ?? throw new ArgumentNullException(nameof(currencies));
            _stringBuilder = stringBuilder ?? new();
        }

        private static CultureInfo P => FormattingHelpers.FormatProvider;

        public string Format(AssetClass assetClass, MulticurrencyAmount multicurrencyAmount)
        {
            _stringBuilder.Clear();
            IReadOnlyDictionary<string, CurrencyAmount> currencyAmountByCurrency =
                multicurrencyAmount.CurrencyAmountByCurrency;
            UncheckedFormat(assetClass, currencyAmountByCurrency);
            string result = _stringBuilder.ToString();
            _stringBuilder.Clear();
            return result;
        }

        private void UncheckedFormat(AssetClass assetClass,
            IReadOnlyDictionary<string, CurrencyAmount> currencyAmountByCurrency)
        {
            int assetClassLength = AppendAssetClass(_stringBuilder, 5, assetClass);
            _stringBuilder.Append(Separator);
            CurrencyAmount assetClassTotal = currencyAmountByCurrency.Values.Aggregate(
                CurrencyAmountMonoid.Instance.Identity, Combine);
            _ = AppendTotalAmount(_stringBuilder, 18 - assetClassLength - Separator.Length, assetClassTotal.Amount);
            _stringBuilder.Append(' ');
            _stringBuilder.Append(assetClassTotal.Currency);
            _stringBuilder.Append(Separator);
            decimal ratio = CurrencyAmountMonoid.Divide(assetClassTotal, _total);
            _stringBuilder.Append(ratio.ToString("P2", P));

            foreach (string currency in _currencies)
            {
                _stringBuilder.Append(Separator);
                if (currencyAmountByCurrency.TryGetValue(currency, out CurrencyAmount currencyAmount))
                {
                    _stringBuilder.Append(currencyAmount.Amount.ToString("F2", P));
                    _stringBuilder.Append(' ');
                    _stringBuilder.Append(currencyAmount.Currency);
                }
                else
                {
                    _stringBuilder.Append("            ");
                }
            }

            CurrencyAmount Combine(CurrencyAmount left, CurrencyAmount right)
            {
                return CurrencyAmountMonoid.Instance.Combine(left, _currencyConverter.ConvertFrom(right));
            }
        }

        private static int AppendAssetClass(StringBuilder stringBuilder, int desiredLength, AssetClass assetClass)
        {
            int initialLength = stringBuilder.Length;
            string raw = assetClass.ToString();
            string padded = raw.Length < desiredLength ? raw.PadRight(desiredLength) : raw;
            stringBuilder.Append(padded);
            return stringBuilder.Length - initialLength;
        }

        private static int AppendTotalAmount(StringBuilder stringBuilder, int desiredLength, decimal totalAmount)
        {
            int initialLength = stringBuilder.Length;
            int bufferLength = Math.Max(desiredLength, 24);
            Span<char> buffer = stackalloc char[bufferLength];
            if (!totalAmount.TryFormat(buffer, out int totalAmountLength, "F2", P))
                return Fallback(totalAmount.ToString("F2", P));

            ReadOnlySpan<char> totalAmountSpan = buffer[..totalAmountLength];
            if (desiredLength <= totalAmountSpan.Length)
                return Fallback(totalAmountSpan);

            int paddingLength = desiredLength - totalAmountSpan.Length;
            Span<char> paddingSpan = buffer.Slice(totalAmountSpan.Length, paddingLength);
            paddingSpan.Fill(' ');
            stringBuilder.Append(paddingSpan);
            stringBuilder.Append(totalAmountSpan);
            return stringBuilder.Length - initialLength;

            int Fallback(ReadOnlySpan<char> valueSpan)
            {
                stringBuilder.Append(valueSpan);
                return stringBuilder.Length - initialLength;
            }
        }
    }
}
