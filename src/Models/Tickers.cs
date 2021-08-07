using System;
using System.Collections.Generic;
using System.Linq;

namespace Diversifolio
{
    public static class Tickers
    {
        public const string Dsky = "DSKY";
        public const string Fxcn = "FXCN";
        public const string Fxde = "FXDE";
        public const string Fxdm = "FXDM";
        public const string Fxes = "FXES";
        public const string Fxgd = "FXGD";
        public const string Fxim = "FXIM";
        public const string Fxip = "FXIP";
        public const string Fxit = "FXIT";
        public const string Fxmm = "FXMM";
        public const string Fxrb = "FXRB";
        public const string Fxrl = "FXRL";
        public const string Fxru = "FXRU";
        public const string Fxrw = "FXRW";
        public const string Fxtb = "FXTB";
        public const string Fxtp = "FXTP";
        public const string Fxus = "FXUS";
        public const string Fxwo = "FXWO";
        public const string Mgnt = "MGNT";
        public const string Mvid = "MVID";
        public const string Rtkm = "RTKM";
        public const string Sbcb = "SBCB";
        public const string Sbgb = "SBGB";
        public const string Sbmx = "SBMX";
        public const string Sbrb = "SBRB";
        public const string Sbsp = "SBSP";
        public const string Su25083 = "SU25083RMFS5";
        public const string Su26214 = "SU26214RMFS5";
        public const string Su29012 = "SU29012RMFS0";
        public const string Tbio = "TBIO";
        public const string Tech = "TECH";
        public const string Tmos = "TMOS";
        public const string Tspx = "TSPX";
        public const string Vtba = "VTBA";
        public const string Vtbb = "VTBB";
        public const string Vtbe = "VTBE";
        public const string Vtbh = "VTBH";
        public const string Vtbu = "VTBU";
        public const string Vtbx = "VTBX";
        public const string Vtby = "VTBY";
        public const string Yndx = "YNDX";

        private static (string, AssetClass, string, string)[]? s_securities;
        private static IDictionary<string, AssetClass>? s_assetClassByTicker;
        private static IDictionary<string, string>? s_marketByTicker;
        private static IDictionary<string, string>? s_boardByTicker;
        private static ILookup<AssetClass, string>? s_tickersByAssetClass;
        private static ILookup<string, string>? s_tickersByMarket;
        private static ILookup<string, string>? s_tickersByBoard;

        private static (string, AssetClass, string, string)[] Securities => s_securities ??= CreateSecurities();

        public static IDictionary<string, AssetClass> AssetClassByTicker => s_assetClassByTicker ??=
            Securities.ToDictionary(it => it.Item1, it => it.Item2, StringComparer.Ordinal);

        public static IDictionary<string, string> MarketByTicker => s_marketByTicker ??=
            Securities.ToDictionary(it => it.Item1, it => it.Item3, StringComparer.Ordinal);

        public static IDictionary<string, string> BoardByTicker => s_boardByTicker ??=
            Securities.ToDictionary(it => it.Item1, it => it.Item4, StringComparer.Ordinal);

        public static ILookup<AssetClass, string> TickersByAssetClass => s_tickersByAssetClass ??=
            Securities.ToLookup(it => it.Item2, it => it.Item1);

        public static ILookup<string, string> TickersByMarket => s_tickersByMarket ??=
            Securities.ToLookup(it => it.Item3, it => it.Item1, StringComparer.Ordinal);

        public static ILookup<string, string> TickersByBoard => s_tickersByBoard ??=
            Securities.ToLookup(it => it.Item4, it => it.Item1, StringComparer.Ordinal);

        private static (string, AssetClass, string, string)[] CreateSecurities() => new[]
        {
            (Dsky, AssetClass.Stock, Markets.Shares, Boards.Tqbr),
            (Fxcn, AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            (Fxde, AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            (Fxdm, AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            (Fxes, AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            (Fxgd, AssetClass.Commodity, Markets.Shares, Boards.Tqtf),
            (Fxim, AssetClass.Stock, Markets.Shares, Boards.Tqtd),
            (Fxip, AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            (Fxit, AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            (Fxmm, AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            (Fxrb, AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            (Fxrl, AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            (Fxru, AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            (Fxrw, AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            (Fxtb, AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            (Fxtp, AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            (Fxus, AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            (Fxwo, AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            (Mgnt, AssetClass.Stock, Markets.Shares, Boards.Tqbr),
            (Mvid, AssetClass.Stock, Markets.Shares, Boards.Tqbr),
            (Rtkm, AssetClass.Stock, Markets.Shares, Boards.Tqbr),
            (Sbcb, AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            (Sbgb, AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            (Sbmx, AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            (Sbrb, AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            (Sbsp, AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            (Su25083, AssetClass.Bond, Markets.Bonds, Boards.Tqob),
            (Su26214, AssetClass.Bond, Markets.Bonds, Boards.Tqob),
            (Su29012, AssetClass.Bond, Markets.Bonds, Boards.Tqob),
            (Tbio, AssetClass.Stock, Markets.Shares, Boards.Tqtd),
            (Tech, AssetClass.Stock, Markets.Shares, Boards.Tqtd),
            (Tmos, AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            (Tspx, AssetClass.Stock, Markets.Shares, Boards.Tqtd),
            (Vtba, AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            (Vtbb, AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            (Vtbe, AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            (Vtbh, AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            (Vtbu, AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            (Vtbx, AssetClass.Stock, Markets.Shares, Boards.Tqtf),
            (Vtby, AssetClass.Bond, Markets.Shares, Boards.Tqtf),
            (Yndx, AssetClass.Stock, Markets.Shares, Boards.Tqbr)
        };
    }
}
