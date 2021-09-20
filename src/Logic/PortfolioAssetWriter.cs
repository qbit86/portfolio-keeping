using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Diversifolio
{
    public sealed class PortfolioAssetWriter
    {
        private static readonly AssetClass[] s_defaultAssetClassesOfInterest = { AssetClass.Stock, AssetClass.Other };
        private static readonly Func<Asset, AssetClass> s_defaultAssetClassSelector = it => it.AssetClass;

        public PortfolioAssetWriter(TextWriter? @out,
            IReadOnlyCollection<AssetClass>? assetClassesOfInterest = null, AssetFormatter? assetFormatter = null)
        {
            AssetClassesOfInterest = assetClassesOfInterest ?? s_defaultAssetClassesOfInterest;
            Out = @out ?? TextWriter.Null;
            AssetFormatter = assetFormatter ?? AssetFormatter.Shared;
        }

        private TextWriter Out { get; }
        private IReadOnlyCollection<AssetClass> AssetClassesOfInterest { get; }
        private AssetFormatter AssetFormatter { get; }

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
            int index = 0;
            foreach (AssetClass key in AssetClassesOfInterest)
            {
                if (index++ > 0)
                    await Out.WriteLineAsync("-----------------------------------------").ConfigureAwait(false);
                IEnumerable<Asset> grouping = assetsByClass[key];
                IOrderedEnumerable<Asset> orderedAssets = grouping
                    .OrderBy(it => it.OriginalPrice.Currency)
                    .ThenByDescending(it => it.Value.Amount);
                foreach (Asset asset in orderedAssets)
                    await Out.WriteLineAsync(AssetFormatter.Format(asset)).ConfigureAwait(false);
            }
        }
    }
}
