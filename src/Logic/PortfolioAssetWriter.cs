using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Diversifolio
{
    public sealed class PortfolioAssetWriter
    {
        private static readonly Func<Asset, AssetClass> s_defaultAssetClassSelector = it => it.AssetClass;

        public PortfolioAssetWriter(TextWriter? @out) => Out = @out ?? TextWriter.Null;

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
            int index = 0;
            IEnumerable<AssetClass> keys = assetsByClass.Select(it => it.Key);
            foreach (AssetClass key in keys)
            {
                if (index++ > 0)
                    await Out.WriteLineAsync("------------------------------------------------").ConfigureAwait(false);
                IEnumerable<Asset> grouping = assetsByClass[key];
                IOrderedEnumerable<Asset> orderedAssets = grouping
                    .OrderBy(it => it.OriginalPrice.Currency)
                    .ThenByDescending(it => it.Value.Amount);
                foreach (Asset asset in orderedAssets)
                    await Out.WriteLineAsync(assetFormatter.Format(asset)).ConfigureAwait(false);
            }
        }
    }
}
