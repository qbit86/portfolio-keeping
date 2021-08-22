using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

[assembly: CLSCompliant(true)]

namespace Diversifolio
{
    internal static class Program
    {
        private static TextWriter Out => Console.Out;

        private static async Task Main()
        {
            string directoryPath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Diversifolio));
            const string portfolioName = PortfolioNames.TinkoffIis;
            await using SqliteConnection connection =
                await DataHelpers.CreatePortfolioConnection(portfolioName, directoryPath).ConfigureAwait(false);

            ImmutableDictionary<string, Position>.Builder builder =
                ImmutableDictionary.CreateBuilder<string, Position>(StringComparer.Ordinal);
            const string commandText = "SELECT Ticker, Balance FROM Position";
            await using SqliteCommand command = new(commandText, connection);
            SqliteDataReader query = command.ExecuteReader();
            while (query.Read())
            {
                string ticker = query.GetString(0);
                decimal balance = query.GetInt32(1);
                Position position = new(ticker, balance);
                builder[ticker] = position;
            }

            ImmutableDictionary<string, Position> positionByTicker = builder.ToImmutable();
            var positions = positionByTicker.Values.OrderBy(it => it.Ticker, StringComparer.Ordinal).ToImmutableArray();
            foreach (Position position in positions)
                await Out.WriteLineAsync(position.ToString()).ConfigureAwait(false);
        }
    }
}
