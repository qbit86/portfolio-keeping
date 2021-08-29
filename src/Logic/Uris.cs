using System;
using System.Collections.Generic;
using System.Linq;
using Diversifolio.Moex;

namespace Diversifolio
{
    public static class Uris
    {
        private static (string, string)[]? s_columnsTuples;
        private static Dictionary<string, string>? s_columnsByTicker;

        public static IReadOnlyDictionary<string, string> ColumnsByTicker => s_columnsByTicker ??=
            ColumnTuples.ToDictionary(it => it.Market, it => it.Column, StringComparer.Ordinal);

        private static (string Market, string Column)[] ColumnTuples => s_columnsTuples ??= CreateColumnTuples();

        private static (string, string)[] CreateColumnTuples() => new[]
        {
            (Markets.Selt, "SECID,BOARDID,SHORTNAME,CURRENCYID,DECIMALS,FACEVALUE,FACEUNIT,PREVPRICE"),
            (Markets.Bonds, "SECID,BOARDID,SHORTNAME,CURRENCYID,DECIMALS,FACEVALUE,LOTSIZE,PREVADMITTEDQUOTE"),
            (Markets.Shares, "SECID,BOARDID,SHORTNAME,CURRENCYID,DECIMALS,FACEVALUE,LOTSIZE,PREVADMITTEDQUOTE")
        };
    }
}
