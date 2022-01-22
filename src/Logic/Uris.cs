using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Diversifolio.Moex;

namespace Diversifolio;

public static class Uris
{
    private static (string, string)[]? s_columnsTuples;
    private static Dictionary<string, Uri>? s_uriByBoard;
    private static Dictionary<string, string>? s_columnsByMarket;

    private static CultureInfo P => CultureInfo.InvariantCulture;

    public static IReadOnlyDictionary<string, Uri> UriByBoard => s_uriByBoard ??=
        Bems.BemByBoard.ToDictionary(it => it.Key, it => BuildUri(it.Value), StringComparer.Ordinal);

    private static IReadOnlyDictionary<string, string> ColumnsByMarket => s_columnsByMarket ??=
        ColumnTuples.ToDictionary(it => it.Market, it => it.Column, StringComparer.Ordinal);

    private static (string Market, string Column)[] ColumnTuples => s_columnsTuples ??= CreateColumnTuples();

    private static Uri BuildUri(Bem bem)
    {
        (string board, string engine, string market) = bem;
        return BuildUri(engine, market, board);
    }

    private static Uri BuildUri(string engine, string market, string board)
    {
        if (!ColumnsByMarket.TryGetValue(market, out string? columns))
            columns = string.Empty;

        IEnumerable<string> tickers = Tickers.TickersByBoard[board];
        string securities = string.Join(",", tickers);

        return BuildUri(engine, market, board, columns, securities);
    }

    private static Uri BuildUri(string engine, string market, string board, string columns, string securities)
    {
        Uri initialUri =
            new($"https://iss.moex.com/iss/engines/{engine}/markets/{market}/boards/{board}/securities.json");
        StringBuilder queryBuilder = new(280);
        queryBuilder.Append("?iss.meta=off").Append("&iss.only=securities");
        if (!string.IsNullOrWhiteSpace(columns))
            queryBuilder.Append(P, $"&securities.columns={columns}");
        if (!string.IsNullOrWhiteSpace(securities))
            queryBuilder.Append(P, $"&securities={securities}");
        UriBuilder uriBuilder = new(initialUri) { Query = queryBuilder.ToString() };
        return uriBuilder.Uri;
    }

    private static (string, string)[] CreateColumnTuples() => new[]
    {
        (Markets.Selt, SeltSecurityFactory.Columns),
        (Markets.Bonds, BondSecurityFactory.Columns),
        (Markets.Shares, ShareSecurityFactory.Columns),
        (Markets.ForeignShares, ShareSecurityFactory.Columns)
    };
}
