﻿using System;
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

            ILookup<bool, Asset> assetsByClass = assets.ToLookup(it => it.AssetClass == AssetClass.Stock);
            for (int i = 0; i < 2; ++i)
            {
                if (i > 0)
                    await Out.WriteLineAsync("-----------------------------------------").ConfigureAwait(false);
                bool key = !Convert.ToBoolean(i);
                IEnumerable<Asset> grouping = assetsByClass[key];
                IOrderedEnumerable<Asset> orderedAssets = grouping
                    .OrderBy(it => it.OriginalPrice.Currency)
                    .ThenByDescending(it => it.Value.Amount);
                foreach (Asset asset in orderedAssets)
                    await Out.WriteLineAsync(AssetFormatter.Shared.Format(asset)).ConfigureAwait(false);
            }
        }
    }
}
