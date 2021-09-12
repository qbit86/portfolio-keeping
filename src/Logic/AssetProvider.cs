using System;
using System.Collections.Generic;
using System.Linq;
using Diversifolio.Moex;

namespace Diversifolio
{
    public readonly struct AssetProvider : IEquatable<AssetProvider>
    {
        private readonly IReadOnlyDictionary<string, IReadOnlyList<Security>> _securitiesByMarket;

        public AssetProvider(IReadOnlyDictionary<string, IReadOnlyList<Security>> securitiesByMarket) =>
            _securitiesByMarket = securitiesByMarket ?? throw new ArgumentNullException(nameof(securitiesByMarket));

        public IReadOnlyList<Asset> GetAssets(IReadOnlyList<Position> positions)
        {
            if (positions is null)
                throw new ArgumentNullException(nameof(positions));

            List<Asset> assets = new(positions.Count);
            PopulateAssets(positions, assets);
            return assets;
        }

        private void PopulateAssets<TCollection>(IReadOnlyList<Position> positions, TCollection assets)
            where TCollection : ICollection<Asset>
        {
            foreach (KeyValuePair<string, IReadOnlyList<Security>> kv in _securitiesByMarket)
            {
                if (SelectAssetFactory(kv.Key) is not { } assetFactory)
                    continue;

                IReadOnlyList<Security> securities = kv.Value;
                IEnumerable<Asset> joinedAssets = from position in positions
                    join security in securities on position.Ticker equals security.SecId
                    select assetFactory.Create(security, position);

                foreach (Asset asset in joinedAssets)
                    assets.Add(asset);
            }
        }

        private static AssetFactory? SelectAssetFactory(string market) =>
            market switch
            {
                Markets.Bonds => BondAssetFactory.Instance,
                Markets.Shares => ShareAssetFactory.Instance,
                _ => default
            };

        public bool Equals(AssetProvider other) => _securitiesByMarket.Equals(other._securitiesByMarket);

        public override bool Equals(object? obj) => obj is AssetProvider other && Equals(other);

        public override int GetHashCode() => _securitiesByMarket.GetHashCode();

        public static bool operator ==(AssetProvider left, AssetProvider right) => left.Equals(right);

        public static bool operator !=(AssetProvider left, AssetProvider right) => !left.Equals(right);
    }
}
