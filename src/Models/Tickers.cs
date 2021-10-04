using System;
using System.Collections.Generic;
using System.Linq;
using Diversifolio.Moex;

namespace Diversifolio
{
    public static class Tickers
    {
        private static (string, AssetClass, string)[]? s_tuples;
        private static Dictionary<string, AssetClass>? s_assetClassByTicker;
        private static Dictionary<string, string>? s_boardByTicker;
        private static ILookup<AssetClass, string>? s_tickersByAssetClass;
        private static ILookup<string, string>? s_tickersByBoard;

        public static IReadOnlyDictionary<string, AssetClass> AssetClassByTicker => s_assetClassByTicker ??=
            Tuples.ToDictionary(it => it.Ticker, it => it.AssetClass, StringComparer.Ordinal);

        public static IReadOnlyDictionary<string, string> BoardByTicker => s_boardByTicker ??=
            Tuples.ToDictionary(it => it.Ticker, it => it.Board, StringComparer.Ordinal);

        public static ILookup<AssetClass, string> TickersByAssetClass => s_tickersByAssetClass ??=
            Tuples.ToLookup(it => it.AssetClass, it => it.Ticker);

        public static ILookup<string, string> TickersByBoard => s_tickersByBoard ??=
            Tuples.ToLookup(it => it.Board, it => it.Ticker, StringComparer.Ordinal);

        private static (string Ticker, AssetClass AssetClass, string Board)[] Tuples => s_tuples ??= CreateTuples();

        private static (string, AssetClass, string)[] CreateTuples() => new[]
        {
            ("DSKY", AssetClass.Stock, Boards.Tqbr),
            ("FXCN", AssetClass.Stock, Boards.Tqtf),
            ("FXDE", AssetClass.Stock, Boards.Tqtf),
            ("FXDM", AssetClass.Stock, Boards.Tqtf),
            ("FXES", AssetClass.Stock, Boards.Tqtf),
            ("FXGD", AssetClass.Commodity, Boards.Tqtf),
            ("FXIM", AssetClass.Stock, Boards.Tqtd),
            ("FXIP", AssetClass.Bond, Boards.Tqtf),
            ("FXIT", AssetClass.Stock, Boards.Tqtf),
            ("FXMM", AssetClass.Bond, Boards.Tqtf),
            ("FXRB", AssetClass.Bond, Boards.Tqtf),
            ("FXRL", AssetClass.Stock, Boards.Tqtf),
            ("FXRU", AssetClass.Bond, Boards.Tqtf),
            ("FXRW", AssetClass.Stock, Boards.Tqtf),
            ("FXTB", AssetClass.Bond, Boards.Tqtf),
            ("FXTP", AssetClass.Bond, Boards.Tqtf),
            ("FXUS", AssetClass.Stock, Boards.Tqtf),
            ("FXWO", AssetClass.Stock, Boards.Tqtf),
            ("MGNT", AssetClass.Stock, Boards.Tqbr),
            ("MVID", AssetClass.Stock, Boards.Tqbr),
            ("RTKM", AssetClass.Stock, Boards.Tqbr),
            ("SBCB", AssetClass.Bond, Boards.Tqtf),
            ("SBGB", AssetClass.Bond, Boards.Tqtf),
            ("SBMX", AssetClass.Stock, Boards.Tqtf),
            ("SBRB", AssetClass.Bond, Boards.Tqtf),
            ("SBSP", AssetClass.Stock, Boards.Tqtf),
            ("SU25083RMFS5", AssetClass.Bond, Boards.Tqob),
            ("SU26214RMFS5", AssetClass.Bond, Boards.Tqob),
            ("SU29012RMFS0", AssetClass.Bond, Boards.Tqob),
            ("TBIO", AssetClass.Stock, Boards.Tqtd),
            ("TECH", AssetClass.Stock, Boards.Tqtd),
            ("TMOS", AssetClass.Stock, Boards.Tqtf),
            ("TSLA-RM", AssetClass.Stock, Boards.Tqbd),
            ("TSPX", AssetClass.Stock, Boards.Tqtd),
            ("USD000UTSTOM", AssetClass.Currency, Boards.Cets),
            ("VTBA", AssetClass.Stock, Boards.Tqtf),
            ("VTBB", AssetClass.Bond, Boards.Tqtf),
            ("VTBE", AssetClass.Stock, Boards.Tqtf),
            ("VTBH", AssetClass.Bond, Boards.Tqtf),
            ("VTBU", AssetClass.Bond, Boards.Tqtf),
            ("VTBX", AssetClass.Stock, Boards.Tqtf),
            ("VTBY", AssetClass.Bond, Boards.Tqtf),
            ("YNDX", AssetClass.Stock, Boards.Tqbr)
        };
    }
}
