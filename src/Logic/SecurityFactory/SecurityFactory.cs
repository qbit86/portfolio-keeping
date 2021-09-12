using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Diversifolio.Moex;
using static System.Diagnostics.Debug;

namespace Diversifolio
{
    public abstract class SecurityFactory
    {
        public abstract bool TryCreate(JsonElement securityElement, [NotNullWhen(true)] out Security? security);

        protected static bool TryGetString(JsonElement arrayElement, int index, [NotNullWhen(true)] out string? result)
        {
            Assert(arrayElement.ValueKind == JsonValueKind.Array);

            result = default;
            int length = arrayElement.GetArrayLength();
            if ((uint)index >= (uint)length)
                return false;

            JsonElement element = arrayElement[index];
            if (element.ValueKind != JsonValueKind.String)
                return false;

            result = element.GetString();
            return result != null;
        }

        protected static bool TryGetInt32(JsonElement arrayElement, int index, out int result)
        {
            Assert(arrayElement.ValueKind == JsonValueKind.Array);

            result = default;
            int length = arrayElement.GetArrayLength();
            if ((uint)index >= (uint)length)
                return false;

            JsonElement element = arrayElement[index];
            if (element.ValueKind == JsonValueKind.Null)
                return false;

            return element.TryGetInt32(out result);
        }

        protected static bool TryGetDecimal(JsonElement arrayElement, int index, out decimal result)
        {
            Assert(arrayElement.ValueKind == JsonValueKind.Array);

            result = default;
            int length = arrayElement.GetArrayLength();
            if ((uint)index >= (uint)length)
                return false;

            JsonElement element = arrayElement[index];
            if (element.ValueKind == JsonValueKind.Null)
                return false;

            return element.TryGetDecimal(out result);
        }
    }
}
