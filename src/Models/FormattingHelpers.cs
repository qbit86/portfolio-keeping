using System;
using System.Globalization;
using System.Text;

namespace Diversifolio
{
    public static class FormattingHelpers
    {
        private static CultureInfo? s_formatProvider;

        public static CultureInfo FormatProvider => s_formatProvider ??= CreateFormatProvider();

        internal static void AppendDecimal(
            StringBuilder stringBuilder, decimal value, string? format, IFormatProvider? formatProvider)
        {
            if (stringBuilder is null)
                throw new ArgumentNullException(nameof(stringBuilder));

            // src/libraries/System.Private.CoreLib/src/System/Number.NumberBuffer.cs
            const int decimalNumberBufferLength = 29 + 1 + 1;
            Span<char> buffer = stackalloc char[decimalNumberBufferLength];
            if (value.TryFormat(buffer, out int charsWritten, format, formatProvider))
                stringBuilder.Append(buffer[..charsWritten]);
            else
                stringBuilder.Append(value.ToString(format, formatProvider));
        }

        internal static int AppendAligned(
            StringBuilder stringBuilder, ReadOnlySpan<char> value, int desiredWidth, bool padLeft = false)
        {
            if (stringBuilder is null)
                throw new ArgumentNullException(nameof(stringBuilder));

            return UncheckedAppendAligned(stringBuilder, value, desiredWidth, padLeft);
        }

        internal static int AppendLeft(StringBuilder stringBuilder, ReadOnlySpan<char> left, int desiredWidth)
        {
            if (stringBuilder is null)
                throw new ArgumentNullException(nameof(stringBuilder));

            return UncheckedAppendAligned(stringBuilder, left, desiredWidth, false);
        }

        internal static int AppendRight(StringBuilder stringBuilder, ReadOnlySpan<char> right, int desiredWidth)
        {
            if (stringBuilder is null)
                throw new ArgumentNullException(nameof(stringBuilder));

            return UncheckedAppendAligned(stringBuilder, right, desiredWidth, true);
        }

        private static int UncheckedAppendAligned(
            StringBuilder stringBuilder, ReadOnlySpan<char> value, int desiredWidth, bool padLeft)
        {
            int initialLength = stringBuilder.Length;
            int paddingWidth = desiredWidth - value.Length;
            if (paddingWidth <= 0)
            {
                stringBuilder.Append(value);
                return ActualWidth();
            }

            Span<char> padding = stackalloc char[paddingWidth];
            padding.Fill(' ');

            if (padLeft)
            {
                stringBuilder.Append(padding);
                stringBuilder.Append(value);
            }
            else
            {
                stringBuilder.Append(value);
                stringBuilder.Append(padding);
            }

            return ActualWidth();

            int ActualWidth()
            {
                return stringBuilder.Length - initialLength;
            }
        }

        private static CultureInfo CreateFormatProvider()
        {
            CultureInfo result = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            result.NumberFormat.PercentPositivePattern = 1;
            result.NumberFormat.PercentNegativePattern = 1;
            return CultureInfo.ReadOnly(result);
        }
    }
}
