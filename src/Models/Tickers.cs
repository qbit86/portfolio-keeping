using System;
using System.Collections.Generic;
using System.Linq;

namespace Diversifolio
{
    public static class Tickers
    {
        private static (string, AssetClass, string, string)[]? s_securities;
        private static Dictionary<string, AssetClass>? s_assetClassByTicker;
        private static Dictionary<string, string>? s_marketByTicker;
        private static Dictionary<string, string>? s_boardByTicker;
        private static ILookup<AssetClass, string>? s_tickersByAssetClass;
        private static ILookup<string, string>? s_tickersByMarket;
        private static ILookup<string, string>? s_tickersByBoard;

        private static (string Ticker, AssetClass AssetClass, string Market, string Board)[] Securities =>
            s_securities ??= CreateSecurities();

        public static IReadOnlyDictionary<string, AssetClass> AssetClassByTicker => s_assetClassByTicker ??=
            Securities.ToDictionary(it => it.Ticker, it => it.AssetClass, StringComparer.Ordinal);

        public static IReadOnlyDictionary<string, string> MarketByTicker => s_marketByTicker ??=
            Securities.ToDictionary(it => it.Ticker, it => it.Market, StringComparer.Ordinal);

        public static IReadOnlyDictionary<string, string> BoardByTicker => s_boardByTicker ??=
            Securities.ToDictionary(it => it.Ticker, it => it.Board, StringComparer.Ordinal);

        public static ILookup<AssetClass, string> TickersByAssetClass => s_tickersByAssetClass ??=
            Securities.ToLookup(it => it.AssetClass, it => it.Ticker);

        public static ILookup<string, string> TickersByMarket => s_tickersByMarket ??=
            Securities.ToLookup(it => it.Market, it => it.Ticker, StringComparer.Ordinal);

        public static ILookup<string, string> TickersByBoard => s_tickersByBoard ??=
            Securities.ToLookup(it => it.Board, it => it.Ticker, StringComparer.Ordinal);

        private static (string, AssetClass, string, string)[] CreateSecurities() => new[]
        {
            ("DSKY", AssetClass.Stock, Markets.Shares, Boards.Tqbr),
            ("FXCN", AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            ("FXDE", AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            ("FXDM", AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            ("FXES", AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            ("FXGD", AssetClass.Commodity, Markets.Shares, Boards.Tqtf),
            ("FXIM", AssetClass.Stock, Markets.Shares, Boards.Tqtd),
            ("FXIP", AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            ("FXIT", AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            ("FXMM", AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            ("FXRB", AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            ("FXRL", AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            ("FXRU", AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            ("FXRW", AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            ("FXTB", AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            ("FXTP", AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            ("FXUS", AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            ("FXWO", AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            ("MGNT", AssetClass.Stock, Markets.Shares, Boards.Tqbr),
            ("MVID", AssetClass.Stock, Markets.Shares, Boards.Tqbr),
            ("RTKM", AssetClass.Stock, Markets.Shares, Boards.Tqbr),
            ("SBCB", AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            ("SBGB", AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            ("SBMX", AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            ("SBRB", AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            ("SBSP", AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            ("SU25083RMFS5", AssetClass.Bond, Markets.Bonds, Boards.Tqob),
            ("SU26214RMFS5", AssetClass.Bond, Markets.Bonds, Boards.Tqob),
            ("SU29012RMFS0", AssetClass.Bond, Markets.Bonds, Boards.Tqob),
            ("TBIO", AssetClass.Stock, Markets.Shares, Boards.Tqtd),
            ("TECH", AssetClass.Stock, Markets.Shares, Boards.Tqtd),
            ("TMOS", AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            ("TSPX", AssetClass.Stock, Markets.Shares, Boards.Tqtd),
            ("USD000UTSTOM", AssetClass.Currency, Markets.Selt, Boards.Cets),
            ("VTBA", AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            ("VTBB", AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            ("VTBE", AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            ("VTBH", AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            ("VTBU", AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            ("VTBX", AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            ("VTBY", AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            ("YNDX", AssetClass.Stock, Markets.Shares, Boards.Tqbr)
        };
    }
}
