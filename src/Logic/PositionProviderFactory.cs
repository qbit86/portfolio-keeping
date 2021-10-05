using System;
using Tinkoff.Trading.OpenApi.Models;

namespace Diversifolio
{
    public static class PositionProviderFactory
    {
        public static PositionProvider Create(string portfolioName)
        {
            if (portfolioName is null)
                throw new ArgumentNullException(portfolioName);

            return portfolioName switch
            {
                PortfolioNames.Tinkoff =>
                    new TinkoffPositionProvider(portfolioName, BrokerAccountType.Tinkoff, "token-tinkoff.txt"),
                PortfolioNames.TinkoffIis =>
                    new TinkoffPositionProvider(portfolioName, BrokerAccountType.TinkoffIis, "token-tinkoff.txt"),
                _ => new DatabasePositionProvider(portfolioName)
            };
        }
    }
}
