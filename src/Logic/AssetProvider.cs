using System;
using System.Collections.Generic;
using System.Linq;
using Diversifolio.Moex;

namespace Diversifolio
{
    public readonly ref struct AssetProvider
    {
        private readonly IReadOnlyDictionary<string, IReadOnlyList<Security>> _securitiesByMarket;

        public AssetProvider(IReadOnlyDictionary<string, IReadOnlyList<Security>> securitiesByMarket) =>
            _securitiesByMarket = securitiesByMarket ?? throw new ArgumentNullException(nameof(securitiesByMarket));

        public ILookup<AssetClass, Asset> GetAssetsByAssetClassLookup(
            IReadOnlyDictionary<string, IReadOnlyList<Security>> securitiesByMarket)
        {
            if (securitiesByMarket is null)
                throw new ArgumentNullException(nameof(securitiesByMarket));

            throw new NotImplementedException();
        }
    }
}
