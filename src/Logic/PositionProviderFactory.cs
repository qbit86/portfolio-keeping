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

            if (portfolioName == PortfolioNames.Tinkoff)
                return new TinkoffPositionProvider(portfolioName, BrokerAccountType.Tinkoff);

            if (portfolioName == PortfolioNames.TinkoffIis)
                return new TinkoffPositionProvider(portfolioName, BrokerAccountType.TinkoffIis);

            return new DatabasePositionProvider(portfolioName);
        }
    }
}
