using System.Collections.Generic;
using System.Linq;

namespace Diversifolio
{
    internal static class Tickers
    {
        internal const string Dsky = "DSKY";
        internal const string Fxcn = "FXCN";
        internal const string Fxde = "FXDE";
        internal const string Fxdm = "FXDM";
        internal const string Fxes = "FXES";
        internal const string Fxgd = "FXGD";
        internal const string Fxim = "FXIM";
        internal const string Fxip = "FXIP";
        internal const string Fxit = "FXIT";
        internal const string Fxmm = "FXMM";
        internal const string Fxrb = "FXRB";
        internal const string Fxrl = "FXRL";
        internal const string Fxru = "FXRU";
        internal const string Fxrw = "FXRW";
        internal const string Fxtb = "FXTB";
        internal const string Fxtp = "FXTP";
        internal const string Fxus = "FXUS";
        internal const string Fxwo = "FXWO";
        internal const string Mgnt = "MGNT";
        internal const string Mvid = "MVID";
        internal const string Rtkm = "RTKM";
        internal const string Sbcb = "SBCB";
        internal const string Sbgb = "SBGB";
        internal const string Sbmx = "SBMX";
        internal const string Sbrb = "SBRB";
        internal const string Sbsp = "SBSP";
        internal const string Su25083 = "SU25083RMFS5";
        internal const string Su26214 = "SU26214RMFS5";
        internal const string Su29012 = "SU29012RMFS0";
        internal const string Tbio = "TBIO";
        internal const string Tech = "TECH";
        internal const string Tmos = "TMOS";
        internal const string Tspx = "TSPX";
        internal const string Vtba = "VTBA";
        internal const string Vtbb = "VTBB";
        internal const string Vtbe = "VTBE";
        internal const string Vtbh = "VTBH";
        internal const string Vtbu = "VTBU";
        internal const string Vtbx = "VTBX";
        internal const string Vtby = "VTBY";
        internal const string Yndx = "YNDX";

        private static (string, AssetClass, string, string)[]? s_securities;
        private static IDictionary<string, AssetClass>? s_assetClassByTicker;
        private static IDictionary<string, string>? s_marketByTicker;
        private static IDictionary<string, string>? s_boardByTicker;

        private static (string, AssetClass, string, string)[] Securities => s_securities ??= CreateSecurities();

        internal static IDictionary<string, AssetClass> AssetClassByTicker =>
            s_assetClassByTicker ??= Securities.ToDictionary(it => it.Item1, it => it.Item2);

        internal static IDictionary<string, string> MarketByTicker =>
            s_marketByTicker ??= Securities.ToDictionary(it => it.Item1, it => it.Item3);

        internal static IDictionary<string, string> BoardByTicker =>
            s_boardByTicker ??= Securities.ToDictionary(it => it.Item1, it => it.Item4);

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
