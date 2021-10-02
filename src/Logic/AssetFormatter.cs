using System;
using System.Globalization;
using System.Text;

namespace Diversifolio
{
    internal sealed class AssetFormatter
    {
        private const string Separator = " | ";

        private readonly StringBuilder _stringBuilder;

        public AssetFormatter(StringBuilder? stringBuilder = null) => _stringBuilder = stringBuilder ?? new();

        public static AssetFormatter Shared { get; } = new(new());

        private static CultureInfo P => CultureInfo.InvariantCulture;

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
            int tickerAndValueLength = AppendTickerAndValue(stringBuilder, asset, 4, 9);

            stringBuilder.Append(Separator);
            _ = AppendBalanceAndPrice(stringBuilder, asset, 23 - tickerAndValueLength - Separator.Length, 9);

            stringBuilder.Append(' ');
            stringBuilder.Append(asset.OriginalPrice.Currency);
        }

        private static int AppendTickerAndValue(
            StringBuilder stringBuilder, Asset asset, int desiredLeftWidth, int desiredRightWidth)
        {
            const string f = "F2";
            string ticker = asset.Ticker;
            decimal value = asset.Value.Amount;

            ReadOnlySpan<char> left = ticker;
            Span<char> buffer = stackalloc char[16];
            ReadOnlySpan<char> right = value.TryFormat(buffer, out int rightLength, f, P)
                ? buffer[..rightLength]
                : value.ToString(f, P);
            return FormattingHelpers.AppendJustified(
                stringBuilder, Separator, left, desiredLeftWidth, right, desiredRightWidth);
        }

        private static int AppendBalanceAndPrice(
            StringBuilder stringBuilder, Asset asset, int desiredLeftWidth, int desiredRightWidth)
        {
            int decimalCount = asset.DecimalCount;
            string f = GetPriceFormat(decimalCount);
            decimal price = asset.OriginalPrice.Amount;

            Span<char> buffer = stackalloc char[24];
            ReadOnlySpan<char> left = asset.Balance.TryFormat(buffer, out int leftLength, "D", P)
                ? buffer[..leftLength]
                : asset.Balance.ToString("D", P);

            buffer = buffer[leftLength..];

            int rawPricePadding = decimalCount > 0 ? 4 - decimalCount : 5;
            int maxPricePadding = Math.Max(0, desiredLeftWidth + desiredRightWidth - left.Length);
            int pricePadding = Math.Clamp(rawPricePadding, 0, maxPricePadding);
            if (price.TryFormat(buffer[..^pricePadding], out int rightLength, f, P))
            {
                buffer.Slice(rightLength, pricePadding).Fill(' ');
                ReadOnlySpan<char> right = buffer[..(rightLength + pricePadding)];
                return FormattingHelpers.AppendJustified(
                    stringBuilder, Separator, left, desiredLeftWidth, right, desiredRightWidth);
            }
            else
            {
                string rawRight = price.ToString(f, P);
                ReadOnlySpan<char> right = rawRight.PadRight(rawRight.Length + pricePadding);
                return FormattingHelpers.AppendJustified(
                    stringBuilder, Separator, left, desiredLeftWidth, right, desiredRightWidth);
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
