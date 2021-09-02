using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

namespace Diversifolio
{
    public sealed record TinkoffPositionProvider(string PortfolioName, BrokerAccountType BrokerAccountType) :
        PositionProvider(PortfolioName)
    {
        private static CultureInfo F => CultureInfo.InvariantCulture;

        protected override async Task PopulatePositionsAsync<TDictionary>(TDictionary positionByTicker)
        {
            if (positionByTicker is null)
                throw new ArgumentNullException(nameof(positionByTicker));

            using var reader = new StreamReader("token-tinkoff.txt");
            string? token = await reader.ReadLineAsync().ConfigureAwait(false);

            // https://github.com/TinkoffCreditSystems/invest-openapi-csharp-sdk
            using Connection connection = ConnectionFactory.GetConnection(token);
            Context context = connection.Context;

            IReadOnlyCollection<Account> accounts = await context.AccountsAsync().ConfigureAwait(false);
            Account account = accounts.Single(it => it.BrokerAccountType == BrokerAccountType);
            Portfolio portfolio = await context.PortfolioAsync(account.BrokerAccountId).ConfigureAwait(false);
            foreach (Portfolio.Position position in portfolio.Positions)
                positionByTicker[position.Ticker] = new(position.Ticker, position.Balance);

            await WriteScriptAsync(portfolio.Positions).ConfigureAwait(false);
        }

        private async Task WriteScriptAsync(List<Portfolio.Position> positions)
        {
            string directoryPath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Diversifolio));
            string scriptPath = Path.Join(directoryPath, PortfolioName + ".sql");
            await using var fileStream = new FileStream(scriptPath, FileMode.Create, FileAccess.Write, FileShare.None);
            await using var writer = new StreamWriter(fileStream, Encoding.UTF8);
            await writer.WriteLineAsync("INSERT INTO Position (Ticker, Balance)").ConfigureAwait(false);
            for (int i = 0; i < positions.Count; ++i)
            {
                Portfolio.Position position = positions[i];
                if (position.Ticker.Contains('\'', StringComparison.Ordinal))
                    throw new InvalidOperationException("The ticker must not contain the apostrophe.");
                bool isConsequent = i > 0;
                string delimiter = isConsequent ? ",\r\n       " : "VALUES ";
                await writer.WriteAsync(delimiter).ConfigureAwait(false);
                string value = $"('{position.Ticker}', {Convert.ToInt32(Math.Floor(position.Balance)).ToString(F)})";
                await writer.WriteAsync(value).ConfigureAwait(false);
            }
        }
    }
}
