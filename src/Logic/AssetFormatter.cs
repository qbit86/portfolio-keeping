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
            int tickerAndValueLength = AppendTickerAndValue(stringBuilder, 16, asset);

            stringBuilder.Append(Separator);
            _ = AppendBalanceAndPrice(stringBuilder, Math.Max(32 - tickerAndValueLength, 3), asset);

            stringBuilder.Append(Separator);
            stringBuilder.Append(asset.OriginalPrice.Currency);
        }

        private static int AppendTickerAndValue(StringBuilder stringBuilder, int desiredLength, Asset asset)
        {
            const string f = "F2";
            string ticker = asset.Ticker;
            decimal value = asset.Value.Amount;
            int initialLength = stringBuilder.Length;

            Span<char> buffer = stackalloc char[24];
            if (!value.TryFormat(buffer, out int valueLength, f, P))
                return Fallback(value.ToString(f, P));

            ReadOnlySpan<char> valueView = buffer[..valueLength];

            int paddedSeparatorLength = desiredLength - ticker.Length - valueView.Length;
            if (paddedSeparatorLength < Separator.Length)
                return Fallback(valueView);

            Span<char> paddedSeparator = stackalloc char[paddedSeparatorLength];
            paddedSeparator.Fill(' ');

            int offset = Math.Clamp(4 - ticker.Length, 0, paddedSeparator.Length - Separator.Length);
            Separator.AsSpan().CopyTo(paddedSeparator[offset..]);

            stringBuilder.Append(ticker);
            stringBuilder.Append(paddedSeparator);
            stringBuilder.Append(valueView);
            return stringBuilder.Length - initialLength;

            int Fallback(ReadOnlySpan<char> valueSpan)
            {
                stringBuilder.Append(ticker);
                stringBuilder.Append(Separator);
                stringBuilder.Append(valueSpan);
                return stringBuilder.Length - initialLength;
            }
        }

        private static int AppendBalanceAndPrice(StringBuilder stringBuilder, int desiredLength, Asset asset)
        {
            int decimalCount = asset.DecimalCount;
            string f = GetPriceFormat(decimalCount);
            decimal price = asset.OriginalPrice.Amount;
            int initialLength = stringBuilder.Length;

            Span<char> remainingBuffer = stackalloc char[40];

            if (!asset.Balance.TryFormat(remainingBuffer, out int balanceLength, "D", P))
                return Fallback(asset.Balance.ToString("D", P), price.ToString(f, P));

            ReadOnlySpan<char> balanceView = remainingBuffer[..balanceLength];
            remainingBuffer = remainingBuffer[balanceLength..];

            if (!price.TryFormat(remainingBuffer, out int rawPriceLength, f, P))
                return Fallback(balanceView, price.ToString(f, P));

            int pricePadding = Math.Clamp(
                decimalCount > 0 ? 4 - decimalCount : 5, 0, remainingBuffer.Length - rawPriceLength);
            remainingBuffer.Slice(rawPriceLength, pricePadding).Fill(' ');
            int priceLength = Math.Clamp(
                desiredLength - balanceLength - Separator.Length, rawPriceLength, rawPriceLength + pricePadding);
            ReadOnlySpan<char> priceView = remainingBuffer[..priceLength];

            int paddedSeparatorLength = desiredLength - balanceView.Length - priceView.Length;
            if (paddedSeparatorLength < Separator.Length)
                return Fallback(balanceView, priceView[..rawPriceLength]);

            Span<char> paddedSeparator = stackalloc char[paddedSeparatorLength];
            paddedSeparator.Fill(' ');

            int offset = Math.Clamp(4 - balanceView.Length, 0, paddedSeparator.Length - Separator.Length);
            Separator.AsSpan().CopyTo(paddedSeparator[offset..]);

            stringBuilder.Append(balanceView);
            stringBuilder.Append(paddedSeparator);
            stringBuilder.Append(priceView);
            return stringBuilder.Length - initialLength;

            int Fallback(ReadOnlySpan<char> balanceSpan, ReadOnlySpan<char> priceSpan)
            {
                stringBuilder.Append(balanceSpan);
                stringBuilder.Append(Separator);
                stringBuilder.Append(priceSpan);
                return stringBuilder.Length - initialLength;
            }

            static string GetPriceFormat(int decimalCount)
            {
                return decimalCount switch
                {
                    0 => "F0",
                    1 => "F1",
                    2 => "F2",
                    3 => "F3",
                    4 => "F4",
                    _ => "F" + decimalCount.ToString(P)
                };
            }
        }
    }
}
