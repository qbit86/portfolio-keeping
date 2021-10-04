using System;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;

namespace Diversifolio
{
    public sealed record CachingTinkoffPositionProvider(string PortfolioName, BrokerAccountType BrokerAccountType) :
        PositionProvider(PortfolioName)
    {
        protected override Task PopulatePositionsAsync<TCollection>(TCollection positions) =>
            throw new NotImplementedException();
    }
}
