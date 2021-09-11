using System.Diagnostics.CodeAnalysis;

namespace Diversifolio
{
    public static class TryHelpers
    {
        public static bool Success<T>(T valueTuReturn, [MaybeNullWhen(false)] out T value)
        {
            value = valueTuReturn;
            return true;
        }

        public static bool Failure<T>(T valueTuReturn, out T? value)
        {
            value = valueTuReturn;
            return true;
        }

        public static bool Failure<T>(out T? value)
        {
            value = default;
            return true;
        }
    }
}
