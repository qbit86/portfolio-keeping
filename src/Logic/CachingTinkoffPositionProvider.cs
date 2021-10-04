using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Tinkoff.Trading.OpenApi.Models;

namespace Diversifolio
{
    public sealed record CachingTinkoffPositionProvider(string PortfolioName, BrokerAccountType BrokerAccountType) :
        PositionProvider(PortfolioName)
    {
        protected override async Task PopulatePositionsAsync<TCollection>(TCollection positions)
        {
            if (positions is null)
                throw new ArgumentNullException(nameof(positions));

            string directoryPath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Diversifolio));
            await using SqliteConnection connection =
                await CreatePortfolioConnectionAsync(PortfolioName, directoryPath).ConfigureAwait(false);

            const string commandText = "SELECT Ticker, Balance FROM Position";
            await using SqliteCommand command = new(commandText, connection);
            SqliteDataReader query = command.ExecuteReader();
            while (query.Read())
            {
                string ticker = query.GetString(0);
                decimal balance = query.GetInt32(1);
                Position position = new(ticker, balance);
                positions.Add(position);
            }
        }

        private async Task<SqliteConnection> CreatePortfolioConnectionAsync(string portfolioName, string directoryPath)
        {
            Directory.CreateDirectory(directoryPath);
            string databasePath = Path.Join(directoryPath, portfolioName + ".db");
            if (!File.Exists(databasePath))
                await CreatePortfolioDatabaseAsync(portfolioName, directoryPath, databasePath).ConfigureAwait(false);

            SqliteConnectionStringBuilder connectionStringBuilder = new()
            {
                DataSource = databasePath,
                Mode = SqliteOpenMode.ReadOnly
            };
            string connectionString = connectionStringBuilder.ToString();
            SqliteConnection connection = new(connectionString);
            connection.Open();
            return connection;
        }

        internal static async Task CreatePortfolioDatabaseAsync(
            string portfolioName, string directoryPath, string databasePath) =>
            throw new NotImplementedException();
    }
}
