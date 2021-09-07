using System;
using System.Text;

namespace Diversifolio
{
    internal static class FormattingHelpers
    {
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
    }
}
