using System;
using System.Diagnostics.CodeAnalysis;
using Diversifolio.Moex;
using static Diversifolio.TryHelpers;

namespace Diversifolio
{
    public sealed class ShareAssetFactory : AssetFactory
    {
        private ShareAssetFactory() { }

        public static ShareAssetFactory Instance { get; } = new();

        protected override bool TryUncheckedCreate(
            Security security, Position position, AssetClass assetClass, [NotNullWhen(true)] out Asset? asset) =>
            security is ShareSecurity shareSecurity
                ? Success(UncheckedCreateShare(shareSecurity, position, assetClass), out asset)
                : Failure(out asset);

        private static Asset UncheckedCreateShare(ShareSecurity security, Position position, AssetClass assetClass)
        {
            int balance = Convert.ToInt32(Math.Floor(position.Balance));
            CurrencyAmount price = new(security.CurrencyId, security.PrevAdmittedQuote);
            CurrencyAmount value = CurrencyAmountMonoid.Multiply(position.Balance, price);
            int decimalCount = security.Decimals;
            return new(
                security.SecId, assetClass, security.BoardId, balance, price, value, decimalCount, security.LotSize);
        }
    }
}
