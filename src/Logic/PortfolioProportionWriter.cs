using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Diversifolio
{
    internal static class PortfolioProportionWriter
    {
        private static readonly AssetClass[] s_defaultAssetClassesOfInterest = { AssetClass.Stock, AssetClass.Other };

        internal static readonly Func<Asset, AssetClass> DefaultAssetClassSelector = it => it.AssetClass;

        internal static IReadOnlyList<AssetClass> DefaultAssetClassesOfInterest => s_defaultAssetClassesOfInterest;
    }

    public sealed class PortfolioProportionWriter<TCurrencyConverter>
        where TCurrencyConverter : ICurrencyConverter
    {
        private readonly TCurrencyConverter _currencyConverter;

        public PortfolioProportionWriter(TCurrencyConverter currencyConverter,
            TextWriter? @out, IReadOnlyCollection<AssetClass>? assetClassesOfInterest = null)
        {
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            AssetClassesOfInterest = assetClassesOfInterest ?? PortfolioProportionWriter.DefaultAssetClassesOfInterest;
            Out = @out ?? TextWriter.Null;
        }

        private static CultureInfo P => CultureInfo.InvariantCulture;

        private TextWriter Out { get; }
        private IReadOnlyCollection<AssetClass> AssetClassesOfInterest { get; }

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
                assets.ToLookup(assetClassSelector ?? PortfolioProportionWriter.DefaultAssetClassSelector);
            return UncheckedWriteAsync(assetsByClass);
        }

        private async Task UncheckedWriteAsync(ILookup<AssetClass, Asset> assetsByClass)
        {
            MulticurrencyAmount totalMulticurrencyAmount = new();
            Dictionary<AssetClass, MulticurrencyAmount> multicurrencyAmountByAssetClass = new();
            foreach (IGrouping<AssetClass, Asset> grouping in assetsByClass)
            {
                MulticurrencyAmount assetClassMulticurrencyAmount = new();
                multicurrencyAmountByAssetClass[grouping.Key] = assetClassMulticurrencyAmount;
                foreach (Asset asset in grouping)
                {
                    totalMulticurrencyAmount.Add(asset.OriginalValue);
                    assetClassMulticurrencyAmount.Add(asset.OriginalValue);
                }
            }

            IReadOnlyDictionary<string, CurrencyAmount> currencyAmountByCurrency =
                totalMulticurrencyAmount.CurrencyAmountByCurrency;

            if (currencyAmountByCurrency.Count == 1)
            {
                decimal totalValue = currencyAmountByCurrency.Single().Value.Amount;
                await Out.WriteLineAsync($"Total\t| {totalValue.ToString("F2", P),10}")
                    .ConfigureAwait(false);
            }
        }
    }
}
