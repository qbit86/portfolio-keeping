using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Diversifolio.Moex;

namespace Diversifolio;

public sealed class ShareSecurityFactory : SecurityFactory
{
    public const string Columns = "SECID,BOARDID,SHORTNAME,CURRENCYID,DECIMALS,FACEVALUE,LOTSIZE,PREVADMITTEDQUOTE";

    private ShareSecurityFactory() { }

    public static ShareSecurityFactory Instance { get; } = new();

    private static bool TryCreate(JsonElement securityElement, [NotNullWhen(true)] out ShareSecurity? security)
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

        if (!TryGetInt32(securityElement, 6, out int lotSize))
            return false;

        if (!TryGetDecimal(securityElement, 7, out decimal price))
            return false;

        security = new(secId, boardId, shortName, currencyId, decimals, faceValue, lotSize, price);
        return true;
    }

    public override bool TryCreate(JsonElement securityElement, [NotNullWhen(true)] out Security? security)
    {
        bool result = TryCreate(securityElement, out ShareSecurity? shareSecurity);
        security = shareSecurity;
        return result;
    }
}
