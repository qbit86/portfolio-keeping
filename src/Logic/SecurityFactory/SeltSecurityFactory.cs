using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Diversifolio.Moex;

namespace Diversifolio
{
    public sealed class SeltSecurityFactory : SecurityFactory
    {
        public const string Columns = "SECID,BOARDID,SHORTNAME,CURRENCYID,DECIMALS,FACEVALUE,FACEUNIT,PREVPRICE";

        private SeltSecurityFactory() { }

        public static SeltSecurityFactory Instance { get; } = new();

        private static bool TryCreate(JsonElement securityElement, [NotNullWhen(true)] out SeltSecurity? security)
        {
            security = default;
            if (securityElement.ValueKind != JsonValueKind.Array)
                return false;

            if (!TryGetString(securityElement, 0, out string? secId))
                return false;

            if (!TryGetString(securityElement, 1, out string? boardId))
                return false;

            if (!TryGetString(securityElement, 2, out string? shortName))
                return false;

            if (!TryGetString(securityElement, 3, out string? currencyId))
                return false;

            if (!TryGetInt32(securityElement, 4, out int decimals))
                return false;

            if (!TryGetDecimal(securityElement, 5, out decimal faceValue))
                return false;

            if (!TryGetString(securityElement, 6, out string? faceUnit))
                return false;

            if (!TryGetDecimal(securityElement, 7, out decimal price))
                return false;

            security = new(secId, boardId, shortName, currencyId, decimals, faceValue, faceUnit, price);
            return true;
        }

        public override bool TryCreate(JsonElement securityElement, [NotNullWhen(true)] out Security? security)
        {
            bool result = TryCreate(securityElement, out SeltSecurity? seltSecurity);
            security = seltSecurity;
            return result;
        }
    }
}
