using System.Collections.Generic;

namespace Diversifolio
{
    internal sealed class AssetClassComparer : IComparer<AssetClass>
    {
        internal static AssetClassComparer Instance { get; } = new();

        public int Compare(AssetClass x, AssetClass y)
        {
            if (x == y)
                return 0;

            if (x is AssetClass.Stock || y is AssetClass.Other)
                return -1;

            if (x is AssetClass.Other || y is AssetClass.Stock)
                return 1;

            return x.CompareTo((int)y);
        }
    }
}
