using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Diversifolio
{
    public sealed record TinkoffPositionProvider(string PortfolioName) : PositionProvider(PortfolioName)
    {
        protected override async Task PopulatePositions(IDictionary<string, Position> positions)
        {
            if (positions is null)
                throw new ArgumentNullException(nameof(positions));

            string directoryPath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Diversifolio));

            Directory.CreateDirectory(directoryPath);
            string databasePath = Path.Join(directoryPath, PortfolioName + ".db");
            if (!File.Exists(databasePath))
                await DataHelpers.CreatePortfolioDatabase(PortfolioName, directoryPath, databasePath)
                    .ConfigureAwait(false);

            SqliteConnectionStringBuilder connectionStringBuilder = new()
            {
                DataSource = databasePath,
                Mode = SqliteOpenMode.ReadOnly
            };
            string connectionString = connectionStringBuilder.ToString();
            await using SqliteConnection connection = new(connectionString);
            connection.Open();

            const string commandText = "SELECT Ticker, Balance FROM Position";
            await using SqliteCommand command = new(commandText, connection);
            SqliteDataReader query = command.ExecuteReader();
            while (query.Read())
            {
                string ticker = query.GetString(0);
                decimal balance = query.GetInt32(1);
                Position position = new(ticker, balance);
                positions[ticker] = position;
            }
        }
    }
}
