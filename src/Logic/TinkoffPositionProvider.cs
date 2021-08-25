using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

namespace Diversifolio
{
    public sealed record TinkoffPositionProvider(string PortfolioName, BrokerAccountType BrokerAccountType) :
        PositionProvider(PortfolioName)
    {
        protected override async Task PopulatePositions<TDictionary>(TDictionary positions)
        {
            if (positions is null)
                throw new ArgumentNullException(nameof(positions));

            using var reader = new StreamReader("token-tinkoff.txt");
            string? token = await reader.ReadLineAsync().ConfigureAwait(false);

            // https://github.com/TinkoffCreditSystems/invest-openapi-csharp-sdk
            using Connection connection = ConnectionFactory.GetConnection(token);
            Context context = connection.Context;

            IReadOnlyCollection<Account> accounts = await context.AccountsAsync().ConfigureAwait(false);
            Account account = accounts.Single(it => it.BrokerAccountType == BrokerAccountType);
            Portfolio portfolio = await context.PortfolioAsync(account.BrokerAccountId).ConfigureAwait(false);
            foreach (Portfolio.Position position in portfolio.Positions)
                positions[position.Ticker] = new(position.Ticker, position.Balance);
        }
    }
}
