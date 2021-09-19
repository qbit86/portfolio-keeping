using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Diversifolio.Moex;

[assembly: CLSCompliant(true)]

namespace Diversifolio
{
    internal static class Program
    {
        private static readonly AssetClass[] s_assetClasses = { AssetClass.Stock, AssetClass.Other };

        private static CultureInfo P => CultureInfo.InvariantCulture;
        private static TextWriter Out => Console.Out;

        private static async Task Main()
        {
            const string portfolioName = PortfolioNames.Vtb;
            PositionProvider positionProvider = PositionProviderFactory.Create(portfolioName);
            IReadOnlyList<Position> positions = await positionProvider.GetPositionsAsync().ConfigureAwait(false);

            using var securityProvider = SecurityProvider.Create();
            IReadOnlyDictionary<string, IReadOnlyList<Security>> securitiesByMarket =
                await securityProvider.GetSecuritiesByMarketDictionaryAsync().ConfigureAwait(false);

            SeltSecurity usd = securitiesByMarket[Markets.Selt].OfType<SeltSecurity>().Single();
            var currencyConverter = RubCurrencyConverter.Create(usd);

            AssetProvider<RubCurrencyConverter> assetProvider = new(securitiesByMarket, currencyConverter);
            IReadOnlyList<Asset> assets = assetProvider.GetAssets(positions);

            ILookup<AssetClass, Asset> assetsByClass = assets.ToLookup(GetAssetClass);

            for (int i = 0; i < s_assetClasses.Length; ++i)
            {
                if (i > 0)
                    await Out.WriteLineAsync("-----------------------------------------").ConfigureAwait(false);
                AssetClass key = s_assetClasses[i];
                IEnumerable<Asset> grouping = assetsByClass[key];
                IOrderedEnumerable<Asset> orderedAssets = grouping
                    .OrderBy(it => it.OriginalPrice.Currency)
                    .ThenByDescending(it => it.Value.Amount);
                foreach (Asset asset in orderedAssets)
                    await Out.WriteLineAsync(AssetFormatter.Shared.Format(asset)).ConfigureAwait(false);
            }

            decimal totalValue = assets.Sum(it => it.Value.Amount);
            await Out.WriteLineAsync().ConfigureAwait(false);
            foreach (AssetClass key in s_assetClasses)
            {
                decimal sum = assetsByClass[key].Sum(it => it.Value.Amount);
                decimal ratio = sum / totalValue;
                await Out.WriteLineAsync($"{key}\t| {sum.ToString("F2", P),10} | {ratio.ToString("P2", P),8}")
                    .ConfigureAwait(false);
            }

            await Out.WriteLineAsync($"Total\t| {totalValue.ToString("F2", P),10}")
                .ConfigureAwait(false);
        }

        private static AssetClass GetAssetClass(Asset asset) => asset.AssetClass switch
        {
            AssetClass.Stock => AssetClass.Stock,
            _ => AssetClass.Other
        };
    }
}
