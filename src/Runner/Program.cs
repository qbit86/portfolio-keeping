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

            AssetWriter assetWriter = new(Out);
            await assetWriter.WriteAsync(assetsByClass).ConfigureAwait(false);

            await Out.WriteLineAsync().ConfigureAwait(false);
            var portfolioProportionWriter = ProportionWriter.Create(currencyConverter, Out);
            await portfolioProportionWriter.WriteAsync(assetsByClass).ConfigureAwait(false);
        }

        private static AssetClass GetAssetClass(Asset asset) =>
            asset.AssetClass == AssetClass.Stock ? AssetClass.Stock : AssetClass.Other;
    }
}
