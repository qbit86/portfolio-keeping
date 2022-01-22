using System;
using System.Globalization;
using System.Text;
using static System.Diagnostics.Debug;

namespace Diversifolio;

internal sealed class AssetFormatter
{
    private const string Separator = " | ";

    private readonly StringBuilder _stringBuilder;
    private readonly decimal _total;

    public AssetFormatter(decimal total, StringBuilder? stringBuilder = null)
    {
        _total = total;
        _stringBuilder = stringBuilder ?? new();
    }

    private static CultureInfo P => FormattingHelpers.FormatProvider;

    public string Format(Asset asset)
    {
        if (asset is null)
            throw new ArgumentNullException(nameof(asset));

        _stringBuilder.Clear();
        UncheckedFormat(asset);
        string result = _stringBuilder.ToString();
        _stringBuilder.Clear();
        return result;
    }

    private void UncheckedFormat(Asset asset)
    {
        Assert(_stringBuilder.Length == 0);

        _ = AppendTickerAndValue(asset, 4, 9);

        _stringBuilder.Append(Separator);
        int excess = _stringBuilder.Length - 19;
        _ = AppendRatio(asset, 6 - excess);

        _stringBuilder.Append(Separator);
        excess = _stringBuilder.Length - 28;
        _ = AppendBalanceAndPrice(asset, 4 - excess, 9);

        _stringBuilder.Append(' ');
        _stringBuilder.Append(asset.OriginalPrice.Currency);
    }

    private int AppendTickerAndValue(Asset asset, int desiredLeftWidth, int desiredRightWidth)
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
            _stringBuilder, Separator, left, desiredLeftWidth, right, desiredRightWidth);
    }

    private int AppendRatio(Asset asset, int desiredWidth)
    {
        const string f = "P2";
        decimal ratio = asset.Value.Amount / _total;
        Span<char> buffer = stackalloc char[16];
        ReadOnlySpan<char> right = ratio.TryFormat(buffer, out int rightLength, f, P)
            ? buffer[..rightLength]
            : ratio.ToString(f, P);
        return FormattingHelpers.AppendRight(_stringBuilder, right, desiredWidth);
    }

    private int AppendBalanceAndPrice(Asset asset, int desiredLeftWidth, int desiredRightWidth)
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
        if (!price.TryFormat(buffer, out int rawRightLength, f, P))
            return Fallback(left);

        int maxPricePadding = Math.Max(0, desiredLeftWidth + desiredRightWidth - left.Length - rawRightLength);
        int pricePadding = Math.Clamp(rawPricePadding, 0, maxPricePadding);
        if (buffer.Length < rawRightLength + pricePadding)
            return Fallback(left);

        buffer.Slice(rawRightLength, pricePadding).Fill(' ');
        ReadOnlySpan<char> right = buffer[..(rawRightLength + pricePadding)];
        return FormattingHelpers.AppendJustified(
            _stringBuilder, Separator, left, desiredLeftWidth, right, desiredRightWidth);

        int Fallback(ReadOnlySpan<char> left)
        {
            string rawRight = price.ToString(f, P);
            int maxPricePadding = Math.Max(0, desiredLeftWidth + desiredRightWidth - left.Length - rawRight.Length);
            int pricePadding = Math.Clamp(rawPricePadding, 0, maxPricePadding);
            ReadOnlySpan<char> right = rawRight.PadRight(rawRight.Length + pricePadding);
            return FormattingHelpers.AppendJustified(
                _stringBuilder, Separator, left, desiredLeftWidth, right, desiredRightWidth);
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
