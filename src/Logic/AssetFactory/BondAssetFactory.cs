using System;
using System.Diagnostics.CodeAnalysis;
using Diversifolio.Moex;
using static Diversifolio.TryHelpers;

namespace Diversifolio
{
    public sealed class BondAssetFactory : AssetFactory
    {
        private BondAssetFactory() { }

        public static BondAssetFactory Instance { get; } = new();

        protected override bool TryUncheckedCreate(
            Security security, Position position, AssetClass assetClass, [NotNullWhen(true)] out Asset? asset) =>
            security is BondSecurity bondSecurity
                ? Success(UncheckedCreateBond(bondSecurity, position, assetClass), out asset)
                : Failure(out asset);

        private static Asset UncheckedCreateBond(BondSecurity security, Position position, AssetClass assetClass)
        {
            int balance = Convert.ToInt32(Math.Floor(position.Balance));
            CurrencyAmount price = new(security.CurrencyId, security.PrevAdmittedQuote * security.FaceValue / 100m);
            CurrencyAmount value = CurrencyAmountMonoid.Multiply(position.Balance, price);
            int decimalCount = security.Decimals - (int)Math.Log10((double)security.FaceValue) + 2;
            return new(
                security.SecId, assetClass, security.BoardId, balance, price, value, decimalCount, security.LotSize);
        }
    }
}
