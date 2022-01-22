using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Diversifolio
{
    public sealed record DatabasePositionProvider(string PortfolioName) : PositionProvider(PortfolioName)
    {
        protected override async Task PopulatePositionsAsync<TCollection>(TCollection positions)
        {
            if (positions is null)
                throw new ArgumentNullException(nameof(positions));

            string directoryPath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Diversifolio));
            using SqliteConnection connection =
                await DataHelpers.CreatePortfolioConnectionAsync(PortfolioName, directoryPath).ConfigureAwait(false);

            const string commandText = "SELECT Ticker, Balance FROM Position";
            using SqliteCommand command = new(commandText, connection);
            SqliteDataReader query = command.ExecuteReader();
            while (query.Read())
            {
                string ticker = query.GetString(0);
                decimal balance = query.GetInt32(1);
                Position position = new(ticker, balance);
                positions.Add(position);
            }
        }
    }
}
