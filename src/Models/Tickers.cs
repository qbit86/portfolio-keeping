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

        private static (string, AssetClass, string)[]? s_securities;
        private static IDictionary<string, AssetClass>? s_assetClassByTicker;
        private static IDictionary<string, string>? s_boardByTicker;

        private static (string, AssetClass, string)[] Securities => s_securities ??= CreateSecurities();

        internal static IDictionary<string, AssetClass> AssetClassByTicker =>
            s_assetClassByTicker ??= Securities.ToDictionary(it => it.Item1, it => it.Item2);

        internal static IDictionary<string, string> BoardByTicker =>
            s_boardByTicker ??= Securities.ToDictionary(it => it.Item1, it => it.Item3);

        private static (string, AssetClass, string)[] CreateSecurities() => new[]
        {
            (Dsky, AssetClass.Stock, Boards.Tqbr),
            (Fxcn, AssetClass.Stock, Boards.Tqtf),
            (Fxde, AssetClass.Stock, Boards.Tqtf),
            (Fxdm, AssetClass.Stock, Boards.Tqtf),
            (Fxes, AssetClass.Stock, Boards.Tqtf),
            (Fxgd, AssetClass.Stock, Boards.Tqtf),
            (Fxim, AssetClass.Stock, Boards.Tqtd),
            (Fxip, AssetClass.Stock, Boards.Tqtf),
            (Fxit, AssetClass.Stock, Boards.Tqtf),
            (Fxmm, AssetClass.Stock, Boards.Tqtf),
            (Fxrb, AssetClass.Stock, Boards.Tqtf),
            (Fxrl, AssetClass.Stock, Boards.Tqtf),
            (Fxru, AssetClass.Stock, Boards.Tqtf),
            (Fxrw, AssetClass.Stock, Boards.Tqtf),
            (Fxtb, AssetClass.Stock, Boards.Tqtf),
            (Fxtp, AssetClass.Stock, Boards.Tqtf),
            (Fxus, AssetClass.Stock, Boards.Tqtf),
            (Fxwo, AssetClass.Stock, Boards.Tqtf),
            (Mgnt, AssetClass.Stock, Boards.Tqbr),
            (Mvid, AssetClass.Stock, Boards.Tqbr),
            (Rtkm, AssetClass.Stock, Boards.Tqbr),
            (Sbcb, AssetClass.Stock, Boards.Tqtf),
            (Sbgb, AssetClass.Stock, Boards.Tqtf),
            (Sbmx, AssetClass.Stock, Boards.Tqtf),
            (Sbrb, AssetClass.Stock, Boards.Tqtf),
            (Sbsp, AssetClass.Stock, Boards.Tqtf),
            (Tbio, AssetClass.Stock, Boards.Tqtd),
            (Tech, AssetClass.Stock, Boards.Tqtd),
            (Tmos, AssetClass.Stock, Boards.Tqtf),
            (Tspx, AssetClass.Stock, Boards.Tqtd),
            (Vtba, AssetClass.Stock, Boards.Tqtf),
            (Vtbb, AssetClass.Stock, Boards.Tqtf),
            (Vtbe, AssetClass.Stock, Boards.Tqtf),
            (Vtbh, AssetClass.Stock, Boards.Tqtf),
            (Vtbu, AssetClass.Stock, Boards.Tqtf),
            (Vtbx, AssetClass.Stock, Boards.Tqtf),
            (Vtby, AssetClass.Stock, Boards.Tqtf),
            (Yndx, AssetClass.Stock, Boards.Tqbr),
            (Su25083, AssetClass.Bond, Boards.Tqob),
            (Su26214, AssetClass.Bond, Boards.Tqob),
            (Su29012, AssetClass.Bond, Boards.Tqob)
        };
    }
}
