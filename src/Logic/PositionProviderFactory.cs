#define DIVERSIFOLIO_TINKOFF_ENABLED

using System;
#if DIVERSIFOLIO_TINKOFF_ENABLED
using Tinkoff.Trading.OpenApi.Models;
#endif

namespace Diversifolio;

public static class PositionProviderFactory
{
    public static PositionProvider Create(string portfolioName)
    {
        ArgumentNullException.ThrowIfNull(portfolioName);

#if DIVERSIFOLIO_TINKOFF_ENABLED
        return portfolioName switch
        {
            PortfolioNames.Tinkoff =>
                new TinkoffPositionProvider(portfolioName, BrokerAccountType.Tinkoff, "token-tinkoff.txt"),
            PortfolioNames.TinkoffIis =>
                new TinkoffPositionProvider(portfolioName, BrokerAccountType.TinkoffIis, "token-tinkoff.txt"),
            _ => new DatabasePositionProvider(portfolioName)
        };
#else
        return new DatabasePositionProvider(portfolioName);
#endif
    }
}
