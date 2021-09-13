using System;
using System.Globalization;
using System.Text;

namespace Diversifolio
{
    public sealed class AssetFormatter
    {
        private const string Separator = " | ";

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
            AppendTickerValue(stringBuilder, asset.Ticker, asset.Value.Amount.ToString("F2", P));

            stringBuilder.Append(Separator);
            stringBuilder.Append(asset.Balance);
            stringBuilder.Append(Separator);
            stringBuilder.Append(asset.Price.Amount);
            stringBuilder.Append(Separator);
            stringBuilder.Append(asset.Value.Currency);
        }

        private static void AppendTickerValue(StringBuilder stringBuilder, string ticker, string value)
        {
            Span<char> tickerValueBuffer = stackalloc char[16];
            if (!ticker.AsSpan().TryCopyTo(tickerValueBuffer))
            {
                Fallback();
                return;
            }

            if (!Separator.AsSpan().TryCopyTo(tickerValueBuffer[ticker.Length..]))
            {
                Fallback();
            }
            else if (value.Length > tickerValueBuffer.Length - ticker.Length - Separator.Length)
            {
                Fallback();
            }
            else
            {
                value.AsSpan().CopyTo(tickerValueBuffer[^value.Length..]);
                stringBuilder.Append(tickerValueBuffer);
            }

            void Fallback()
            {
                stringBuilder.Append(ticker);
                stringBuilder.Append(Separator);
                stringBuilder.Append(value);
            }
        }
    }
}
