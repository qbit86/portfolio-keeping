using System;
using System.Collections.Generic;
using System.Linq;

namespace Diversifolio.Moex;

public static class Bems
{
    private static (string, string, string)[]? s_tuples;
    private static Dictionary<string, Bem>? s_bemByBoard;
    private static ILookup<string, Bem>? s_bemsByEngine;
    private static ILookup<string, Bem>? s_bemsByMarket;

    public static IReadOnlyDictionary<string, Bem> BemByBoard => s_bemByBoard ??=
        Tuples.ToDictionary(it => it.Board, it => new Bem(it.Board, it.Engine, it.Market), StringComparer.Ordinal);

    public static ILookup<string, Bem> BemsByEngine => s_bemsByEngine ??=
        Tuples.ToLookup(it => it.Engine, it => new Bem(it.Board, it.Engine, it.Market), StringComparer.Ordinal);

    public static ILookup<string, Bem> BemsByMarket => s_bemsByMarket ??=
        Tuples.ToLookup(it => it.Market, it => new Bem(it.Board, it.Engine, it.Market), StringComparer.Ordinal);

    private static (string Board, string Engine, string Market)[] Tuples => s_tuples ??= CreateTuples();

    private static (string, string, string)[] CreateTuples() => new[]
    {
        (Boards.Cets, Engines.Currency, Markets.Selt),
        (Boards.Tqob, Engines.Stock, Markets.Bonds),
        (Boards.Tqbd, Engines.Stock, Markets.ForeignShares),
        (Boards.Tqbr, Engines.Stock, Markets.Shares),
        (Boards.Tqtd, Engines.Stock, Markets.Shares),
        (Boards.Tqtf, Engines.Stock, Markets.Shares)
    };
}
