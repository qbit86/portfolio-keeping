using System;
using System.Linq;

namespace Diversifolio.Moex
{
    public static class Bems
    {
        private static (string, string, string)[]? s_bemTuples;
        private static ILookup<string, Bem>? s_bemsByEngine;

        public static ILookup<string, Bem> BemsByEngine => s_bemsByEngine ??=
            BemTuples.ToLookup(it => it.Engine, it => new Bem(it.Board, it.Engine, it.Market), StringComparer.Ordinal);

        private static (string Board, string Engine, string Market)[] BemTuples => s_bemTuples ??= CreateBemTuples();

        private static (string, string, string)[] CreateBemTuples() => new[]
        {
            (Engines.Currency, Markets.Selt, Boards.Cets),
            (Engines.Stock, Markets.Bonds, Boards.Tqob),
            (Engines.Stock, Markets.Shares, Boards.Tqbr),
            (Engines.Stock, Markets.Shares, Boards.Tqtd),
            (Engines.Stock, Markets.Shares, Boards.Tqtf)
        };
    }
}
