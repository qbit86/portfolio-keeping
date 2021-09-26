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
            // src/libraries/System.Private.CoreLib/src/System/Number.NumberBuffer.cs
            const int decimalNumberBufferLength = 29 + 1 + 1;
            Span<char> buffer = stackalloc char[decimalNumberBufferLength];
            if (value.TryFormat(buffer, out int charsWritten, format, formatProvider))
                stringBuilder.Append(buffer[..charsWritten]);
            else
                stringBuilder.Append(value.ToString(format, formatProvider));
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
