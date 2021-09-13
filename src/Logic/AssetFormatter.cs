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
            _ = AppendTickerValue(stringBuilder, asset);

            stringBuilder.Append(Separator);
            stringBuilder.Append(asset.Balance);
            stringBuilder.Append(Separator);
            stringBuilder.Append(asset.Price.Amount);
            stringBuilder.Append(Separator);
            stringBuilder.Append(asset.Value.Currency);
        }

        private static bool AppendTickerValue(StringBuilder stringBuilder, Asset asset)
        {
            string ticker = asset.Ticker;
            string value = asset.Value.Amount.ToString("F2", P);

            Span<char> destination = stackalloc char[16];
            if (value.Length > destination.Length)
                return Fallback();

            value.AsSpan().CopyTo(destination[^value.Length..]);

            ReadOnlySpan<char> view = destination;
            destination = destination[..^value.Length];
            if (!ticker.AsSpan().TryCopyTo(destination))
                return Fallback();

            destination = destination[ticker.Length..];
            if (!Separator.AsSpan().TryCopyTo(destination))
                return Fallback();

            stringBuilder.Append(view);
            return true;

            bool Fallback()
            {
                stringBuilder.Append(ticker);
                stringBuilder.Append(Separator);
                stringBuilder.Append(value);
                return false;
            }
        }
    }
}
