using System;
using System.Collections.Generic;
using Diversifolio.Moex;

namespace Diversifolio
{
    public readonly ref struct AssetProvider
    {
        private readonly IReadOnlyDictionary<string, IReadOnlyList<Security>> _securitiesByMarket;

        public AssetProvider(IReadOnlyDictionary<string, IReadOnlyList<Security>> securitiesByMarket) =>
            _securitiesByMarket = securitiesByMarket ?? throw new ArgumentNullException(nameof(securitiesByMarket));

        private void PopulateAssets<TCollection>(IReadOnlyList<Position> positions, TCollection assets)
            where TCollection : ICollection<Asset> =>
            throw new NotImplementedException();

        private static AssetFactory? SelectAssetFactory(string market) =>
            market switch
            {
                Markets.Bonds => BondAssetFactory.Instance,
                Markets.Shares => ShareAssetFactory.Instance,
                _ => default
            };
    }
}
