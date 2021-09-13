using System;
using System.Globalization;
using System.Text;

namespace Diversifolio
{
    public sealed class AssetFormatter
    {
        private readonly StringBuilder _stringBuilder;

        public AssetFormatter(StringBuilder stringBuilder) =>
            _stringBuilder = stringBuilder ?? throw new ArgumentNullException(nameof(stringBuilder));

        public static AssetFormatter Shared { get; } = new(new());

        private static CultureInfo P => CultureInfo.InvariantCulture;

        public static void Format(Asset asset, StringBuilder stringBuilder)
        {
            if (asset is null)
                throw new ArgumentNullException(nameof(asset));
            if (stringBuilder is null)
                throw new ArgumentNullException(nameof(stringBuilder));

            UncheckedFormat(asset, stringBuilder);
        }

        public string Format(Asset asset)
        {
            if (asset is null)
                throw new ArgumentNullException(nameof(asset));

            _stringBuilder.Clear();
            UncheckedFormat(asset, _stringBuilder);
            string result = _stringBuilder.ToString();
            _stringBuilder.Clear();
            return result;
        }

        private static void UncheckedFormat(Asset asset, StringBuilder stringBuilder)
        {
            ReadOnlySpan<char> separator = " | ";

            string ticker = asset.Ticker;
            string valueString = asset.Value.Amount.ToString("F2", P);
            Span<char> tickerValueBuffer = stackalloc char[16];
            if (!asset.Ticker.AsSpan().TryCopyTo(tickerValueBuffer))
            {
                AppendTickerValueFallback(ticker, valueString, stringBuilder);
            }
            else if (!separator.TryCopyTo(tickerValueBuffer[ticker.Length..]))
            {
                AppendTickerValueFallback(ticker, valueString, stringBuilder);
            }
            else if (valueString.Length > tickerValueBuffer.Length - ticker.Length - separator.Length)
            {
                AppendTickerValueFallback(ticker, valueString, stringBuilder);
            }
            else
            {
                valueString.AsSpan().CopyTo(tickerValueBuffer[^valueString.Length..]);
                stringBuilder.Append(tickerValueBuffer);
            }

            stringBuilder.Append(separator);
            stringBuilder.Append(asset.Balance);
            stringBuilder.Append(separator);
            stringBuilder.Append(asset.Price.Amount);
            stringBuilder.Append(separator);
            stringBuilder.Append(asset.Value.Currency);
        }

        private static void AppendTickerValueFallback(string ticker, string value, StringBuilder stringBuilder)
        {
            stringBuilder.Append(ticker);
            stringBuilder.Append(" | ");
            stringBuilder.Append(value);
        }
    }
}
