using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Diversifolio
{
    public sealed class AssetWriter
    {
        private static readonly Func<Asset, AssetClass> s_defaultAssetClassSelector = it => it.AssetClass;

        public AssetWriter(TextWriter? @out) => Out = @out ?? TextWriter.Null;

        private TextWriter Out { get; }

        public Task WriteAsync(ILookup<AssetClass, Asset>? assetsByClass)
        {
            if (assetsByClass is null)
                return Task.CompletedTask;

            return UncheckedWriteAsync(assetsByClass);
        }

        public Task WriteAsync(IEnumerable<Asset>? assets, Func<Asset, AssetClass>? assetClassSelector = null)
        {
            if (assets is null)
                return Task.CompletedTask;

            ILookup<AssetClass, Asset> assetsByClass =
                assets.ToLookup(assetClassSelector ?? s_defaultAssetClassSelector);
            return UncheckedWriteAsync(assetsByClass);
        }

        private async Task UncheckedWriteAsync(ILookup<AssetClass, Asset> assetsByClass)
        {
            decimal total = assetsByClass.SelectMany(it => it).Sum(it => it.Value.Amount);
            AssetFormatter assetFormatter = new(total);
            IOrderedEnumerable<IGrouping<AssetClass, Asset>> ordered =
                assetsByClass.OrderBy(it => it.Key, AssetClassComparer.Instance);
            int index = 0;
            foreach (IGrouping<AssetClass, Asset> grouping in ordered)
            {
                if (index++ > 0)
                    await Out.WriteLineAsync("------------------------------------------------").ConfigureAwait(false);
                IOrderedEnumerable<Asset> orderedAssets = grouping
                    .OrderByDescending(it => it.Value.Amount);
                foreach (Asset asset in orderedAssets)
                    await Out.WriteLineAsync(assetFormatter.Format(asset)).ConfigureAwait(false);
            }
        }
    }
}
