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
            _stringBuilder.Append(assetClass.ToString());
            _stringBuilder.Append(Separator);
            CurrencyAmount assetClassTotal = currencyAmountByCurrency.Values.Aggregate(
                CurrencyAmountMonoid.Instance.Identity, Combine);
            _stringBuilder.Append(assetClassTotal.Amount.ToString("F2", P));
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
    }
}
