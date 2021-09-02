using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Diversifolio
{
    public sealed record DatabasePositionProvider(string PortfolioName) : PositionProvider(PortfolioName)
    {
        protected override async Task PopulatePositionsAsync<TDictionary>(TDictionary positionByTicker)
        {
            if (positionByTicker is null)
                throw new ArgumentNullException(nameof(positionByTicker));

            string directoryPath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Diversifolio));
            await using SqliteConnection connection =
                await DataHelpers.CreatePortfolioConnectionAsync(PortfolioName, directoryPath).ConfigureAwait(false);

            const string commandText = "SELECT Ticker, Balance FROM Position";
            await using SqliteCommand command = new(commandText, connection);
            SqliteDataReader query = command.ExecuteReader();
            while (query.Read())
            {
                string ticker = query.GetString(0);
                decimal balance = query.GetInt32(1);
                Position position = new(ticker, balance);
                positionByTicker[ticker] = position;
            }
        }
    }
}
