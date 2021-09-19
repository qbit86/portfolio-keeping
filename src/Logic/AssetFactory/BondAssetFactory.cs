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

        protected override bool TryUncheckedCreate<TCurrencyConverter>(
            Security security, Position position, AssetClass assetClass, TCurrencyConverter currencyConverter,
            [NotNullWhen(true)] out Asset? asset) =>
            security is BondSecurity bondSecurity
                ? Success(UncheckedCreateBond(bondSecurity, position, assetClass, currencyConverter), out asset)
                : Failure(out asset);

        private static Asset UncheckedCreateBond<TCurrencyConverter>(
            BondSecurity security, Position position, AssetClass assetClass, TCurrencyConverter currencyConverter)
            where TCurrencyConverter : ICurrencyConverter
        {
            int balance = Convert.ToInt32(Math.Floor(position.Balance));
            CurrencyAmount originalPrice = new(
                security.CurrencyId, security.PrevAdmittedQuote * security.FaceValue / 100m);
            CurrencyAmount originalValue = CurrencyAmountMonoid.Multiply(position.Balance, originalPrice);
            CurrencyAmount price = currencyConverter.ConvertFrom(originalPrice);
            CurrencyAmount value = currencyConverter.ConvertFrom(originalValue);
            int decimalCount = security.Decimals - (int)Math.Log10((double)security.FaceValue) + 2;
            return new(
                security.SecId, assetClass, security.BoardId, balance, price, value, originalPrice, originalValue,
                decimalCount, security.LotSize);
        }
    }
}
