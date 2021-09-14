using System;
using System.Collections.Generic;
using System.IO;
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

            AssetProvider assetProvider = new(securitiesByMarket);
            IReadOnlyList<Asset> assets = assetProvider.GetAssets(positions);
            foreach (Asset asset in assets)
                await Out.WriteLineAsync(AssetFormatter.Shared.Format(asset)).ConfigureAwait(false);
        }
    }
}
