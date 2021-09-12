using System;
using System.Diagnostics.CodeAnalysis;
using Diversifolio.Moex;

namespace Diversifolio
{
    public abstract class AssetFactory
    {
        protected abstract bool TryUncheckedCreate(
            Security security, Position position, AssetClass assetClass, [NotNullWhen(true)] out Asset? asset);

        public Asset Create(Security security, Position position)
        {
            if (security is null)
                throw new ArgumentNullException(nameof(security));

            if (!string.Equals(position.Ticker, security.SecId, StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    $"{nameof(Security.SecId)} must be equal to {nameof(Position.Ticker)}", nameof(position));
            }

            if (!Tickers.AssetClassByTicker.TryGetValue(position.Ticker, out AssetClass assetClass))
            {
                throw new ArgumentException(
                    $"Could not find asset class for ticker: {position.Ticker}", nameof(position));
            }

            if (!TryUncheckedCreate(security, position, assetClass, out Asset? asset))
                throw new ArgumentException($"Unsupported security type: {security}", nameof(security));

            return asset;
        }
    }
}
