using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Diversifolio.Moex;

[assembly: CLSCompliant(true)]

namespace Diversifolio
{
    internal static class Program
    {
        private static TextWriter Out => Console.Out;

        private static async Task Main()
        {
            const string portfolioName = PortfolioNames.Vtb;

            using var securityProvider = SecurityProvider.Create();
            IReadOnlyDictionary<string, IReadOnlyList<Security>> securitiesByMarket =
                await securityProvider.GetSecuritiesByMarketDictionaryAsync().ConfigureAwait(false);

            SeltSecurity usd = securitiesByMarket[Markets.Selt].OfType<SeltSecurity>().Single();
            var currencyConverter = RubCurrencyConverter.Create(usd);

            AssetProvider<RubCurrencyConverter> assetProvider = new(securitiesByMarket, currencyConverter);

            await PlanAsync(portfolioName, currencyConverter, assetProvider).ConfigureAwait(false);
        }

        private static async Task PlanAsync(string portfolioName, RubCurrencyConverter currencyConverter,
            AssetProvider<RubCurrencyConverter> assetProvider)
        {
            PositionProvider positionProvider = PositionProviderFactory.Create(portfolioName);
            IReadOnlyList<Position> portfolioPositions =
                await positionProvider.GetPositionsAsync().ConfigureAwait(false);

            IReadOnlyList<Asset> portfolioAssets = assetProvider.GetAssets(portfolioPositions);
            ILookup<AssetClass, Asset> portfolioAssetsByClass = portfolioAssets.ToLookup(GetAssetClass);

            AssetWriter assetWriter = new(Out);
            await assetWriter.WriteAsync(portfolioAssetsByClass).ConfigureAwait(false);

            await Out.WriteLineAsync().ConfigureAwait(false);
            var proportionWriter = ProportionWriter.Create(currencyConverter, Out);
            await proportionWriter.WriteAsync(portfolioAssetsByClass).ConfigureAwait(false);
        }

        private static AssetClass GetAssetClass(Asset asset) =>
            asset.AssetClass == AssetClass.Stock ? AssetClass.Stock : AssetClass.Other;
    }
}
