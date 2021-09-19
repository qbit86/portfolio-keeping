using System;
using System.Collections.Generic;
using System.Linq;
using Diversifolio.Moex;

namespace Diversifolio
{
    public readonly struct AssetProvider<TCurrencyConverter> : IEquatable<AssetProvider<TCurrencyConverter>>
        where TCurrencyConverter : ICurrencyConverter
    {
        private readonly IReadOnlyDictionary<string, IReadOnlyList<Security>> _securitiesByMarket;
        private readonly TCurrencyConverter _currencyConverter;

        public AssetProvider(IReadOnlyDictionary<string, IReadOnlyList<Security>> securitiesByMarket,
            TCurrencyConverter currencyConverter)
        {
            _securitiesByMarket = securitiesByMarket ?? throw new ArgumentNullException(nameof(securitiesByMarket));
            _currencyConverter = currencyConverter ?? throw new ArgumentNullException(nameof(currencyConverter));
        }

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

                AssetProvider<TCurrencyConverter> self = this;

                IReadOnlyList<Security> securities = kv.Value;
                IEnumerable<Asset> joinedAssets = from position in positions
                    join security in securities on position.Ticker equals security.SecId
                    select assetFactory.Create(security, position, self._currencyConverter);

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

        public bool Equals(AssetProvider<TCurrencyConverter> other) =>
            _securitiesByMarket.Equals(other._securitiesByMarket) &&
            EqualityComparer<TCurrencyConverter>.Default.Equals(_currencyConverter, other._currencyConverter);

        public override bool Equals(object? obj) => obj is AssetProvider<TCurrencyConverter> other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(_securitiesByMarket, _currencyConverter);

        public static bool operator ==(
            AssetProvider<TCurrencyConverter> left, AssetProvider<TCurrencyConverter> right) => left.Equals(right);

        public static bool operator !=(
            AssetProvider<TCurrencyConverter> left, AssetProvider<TCurrencyConverter> right) => !left.Equals(right);
    }
}
