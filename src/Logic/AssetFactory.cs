using System;
using Diversifolio.Moex;

namespace Diversifolio
{
    public static class AssetFactory
    {
        public static Asset CreateBond(BondSecurity security, Position position)
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

            return UncheckedCreateBond(security, position, assetClass);
        }

        public static Asset CreateShare(ShareSecurity security, Position position)
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

            return UncheckedCreateShare(security, position, assetClass);
        }

        private static Asset UncheckedCreateBond(BondSecurity security, Position position, AssetClass assetClass)
        {
            int balance = Convert.ToInt32(Math.Floor(position.Balance));
            CurrencyAmount price = new(security.CurrencyId, security.PrevAdmittedQuote * security.FaceValue / 100m);
            int decimalCount = security.Decimals - (int)Math.Log10((double)security.FaceValue) + 2;
            return new(security.SecId, assetClass, security.BoardId, balance, price, decimalCount, security.LotSize);
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
