using System;
using Diversifolio.Moex;

namespace Diversifolio
{
    public static class AssetFactory
    {
        public static Asset CreateShare(ShareSecurity security, Position position)
        {
            if (security is null)
                throw new ArgumentNullException(nameof(security));

            if (!string.Equals(position.Ticker, security.SecId, StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    $"{nameof(security.SecId)} must be equal to {nameof(position.Ticker)}", nameof(position));
            }

            if (!Tickers.AssetClassByTicker.TryGetValue(position.Ticker, out AssetClass assetClass))
            {
                throw new ArgumentException(
                    $"Could not find asset class for ticker: {position.Ticker}", nameof(position));
            }

            return UncheckedCreateShare(security, position, assetClass);
        }

        private static Asset UncheckedCreateShare(ShareSecurity security, Position position, AssetClass assetClass)
        {
            int balance = Convert.ToInt32(Math.Floor(position.Balance));
            CurrencyAmount price = new(security.CurrencyId, security.PrevAdmittedQuote);
            int decimalCount = security.Decimals;
            return new(security.SecId, assetClass, security.BoardId, balance, price, decimalCount, security.LotSize);
        }
    }
}
