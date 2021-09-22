using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Diversifolio
{
    public static class PortfolioProportionWriter
    {
        internal static readonly Func<Asset, AssetClass> DefaultAssetClassSelector = it => it.AssetClass;

        public static PortfolioProportionWriter<TCurrencyConverter> Create<TCurrencyConverter>(
            TCurrencyConverter currencyConverter, TextWriter? @out) where TCurrencyConverter : ICurrencyConverter =>
            new(currencyConverter, @out);
    }

    public sealed class PortfolioProportionWriter<TCurrencyConverter>
        where TCurrencyConverter : ICurrencyConverter
    {
        private readonly TCurrencyConverter _currencyConverter;

        public PortfolioProportionWriter(TCurrencyConverter currencyConverter, TextWriter? @out)
        {
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
            Out = @out ?? TextWriter.Null;
        }

        private static CultureInfo P => CultureInfo.InvariantCulture;

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
                assets.ToLookup(assetClassSelector ?? PortfolioProportionWriter.DefaultAssetClassSelector);
            return UncheckedWriteAsync(assetsByClass);
        }

        private async Task UncheckedWriteAsync(ILookup<AssetClass, Asset> assetsByClass)
        {
            var currencies = assetsByClass.SelectMany(it => it.Select(a => a.OriginalValue.Currency))
                .Distinct(StringComparer.Ordinal).OrderBy(it => it, StringComparer.Ordinal).ToList();
            await Out.WriteLineAsync($"{string.Join(", ", currencies)}").ConfigureAwait(false);

            var totalMulticurrencyAmount = MulticurrencyAmount.Create();

            Dictionary<AssetClass, MulticurrencyAmount> multicurrencyAmountByAssetClass = new();
            foreach (IGrouping<AssetClass, Asset> grouping in assetsByClass)
            {
                var assetClassMulticurrencyAmount = MulticurrencyAmount.Create();
                multicurrencyAmountByAssetClass[grouping.Key] = assetClassMulticurrencyAmount;
                foreach (Asset asset in grouping)
                {
                    totalMulticurrencyAmount.Add(asset.OriginalValue);
                    assetClassMulticurrencyAmount.Add(asset.OriginalValue);
                }
            }

            CurrencyAmount totalCurrencyAmount = totalMulticurrencyAmount.CurrencyAmountByCurrency.Values.Aggregate(
                CurrencyAmountMonoid.Instance.Identity, Combine);

            await Out.WriteLineAsync($"Total | {totalCurrencyAmount.Amount.ToString("F2", P)}").ConfigureAwait(false);

            CurrencyAmount Combine(CurrencyAmount left, CurrencyAmount right)
            {
                return CurrencyAmountMonoid.Instance.Combine(left, _currencyConverter.ConvertFrom(right));
            }
        }
    }
}
