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

            AssetProvider assetProvider = new(securitiesByMarket);
            IReadOnlyList<Asset> assets = assetProvider.GetAssets(positions);

            ILookup<bool, Asset> assetsByClass = assets.ToLookup(it => it.AssetClass == AssetClass.Stock);
            for (int i = 0; i < 2; ++i)
            {
                if (i > 0)
                    await Out.WriteLineAsync("-----------------------------------------").ConfigureAwait(false);
                bool key = !Convert.ToBoolean(i);
                IEnumerable<Asset> grouping = assetsByClass[key];
                foreach (Asset asset in grouping)
                    await Out.WriteLineAsync(AssetFormatter.Shared.Format(asset)).ConfigureAwait(false);
            }
        }
    }
}
