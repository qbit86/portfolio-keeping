using System;

namespace Diversifolio
{
    public static class PositionProviderFactory
    {
        public static PositionProvider Create(string portfolioName)
        {
            if (portfolioName is null)
                throw new ArgumentNullException(portfolioName);

            if (portfolioName == PortfolioNames.TinkoffIis)
                return new TinkoffPositionProvider(portfolioName);

            return new DatabasePositionProvider(portfolioName);
        }
    }
}
