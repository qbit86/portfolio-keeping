using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Diversifolio.Moex;

namespace Diversifolio
{
    public static class Uris
    {
        private static (string, string)[]? s_columnsTuples;
        private static Dictionary<string, string>? s_columnsByMarket;

        public static IReadOnlyDictionary<string, string> ColumnsByMarket => s_columnsByMarket ??=
            ColumnTuples.ToDictionary(it => it.Market, it => it.Column, StringComparer.Ordinal);

        private static (string Market, string Column)[] ColumnTuples => s_columnsTuples ??= CreateColumnTuples();

        internal static Uri BuildUri(string engine, string market, string board, string securities)
        {
            if (!ColumnsByMarket.TryGetValue(market, out string? columns))
                columns = string.Empty;

            return BuildUri(engine, market, board, columns, securities);
        }

        private static Uri BuildUri(string engine, string market, string board, string columns, string securities)
        {
            Uri initialUri =
                new($"https://iss.moex.com/iss/engines/{engine}/markets/{market}/boards/{board}/securities.json");
            StringBuilder queryBuilder = new(280);
            queryBuilder.Append("?iss.meta=off").Append("&iss.only=securities");
            if (!string.IsNullOrWhiteSpace(columns))
                queryBuilder.Append($"&securities.columns={columns}");
            if (!string.IsNullOrWhiteSpace(securities))
                queryBuilder.Append($"&securities={securities}");
            UriBuilder uriBuilder = new(initialUri) { Query = queryBuilder.ToString() };
            return uriBuilder.Uri;
        }

        private static (string, string)[] CreateColumnTuples() => new[]
        {
            (Markets.Selt, "SECID,BOARDID,SHORTNAME,CURRENCYID,DECIMALS,FACEVALUE,FACEUNIT,PREVPRICE"),
            (Markets.Bonds, "SECID,BOARDID,SHORTNAME,CURRENCYID,DECIMALS,FACEVALUE,LOTSIZE,PREVADMITTEDQUOTE"),
            (Markets.Shares, "SECID,BOARDID,SHORTNAME,CURRENCYID,DECIMALS,FACEVALUE,LOTSIZE,PREVADMITTEDQUOTE")
        };
    }
}
