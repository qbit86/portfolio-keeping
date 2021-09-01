using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Diversifolio
{
    internal static class DataHelpers
    {
        internal static async Task<SqliteConnection> CreatePortfolioConnectionAsync(
            string portfolioName, string directoryPath)
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
            string portfolioName, string directoryPath, string databasePath)
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new()
            {
                DataSource = databasePath,
                Mode = SqliteOpenMode.ReadWriteCreate
            };
            string connectionString = connectionStringBuilder.ToString();
            await using SqliteConnection connection = new(connectionString);
            connection.Open();
            await CreatePositionTableAsync(connection).ConfigureAwait(false);
            await PopulatePositionTableAsync(connection, portfolioName, directoryPath).ConfigureAwait(false);
        }

        private static async Task CreatePositionTableAsync(SqliteConnection connection)
        {
            var assembly = Assembly.GetExecutingAssembly();
            await using Stream stream = assembly.GetManifestResourceStream(typeof(DataHelpers), "CreatePosition.sql")!;
            using StreamReader reader = new(stream);
            await ExecuteReaderAsync(connection, reader).ConfigureAwait(false);
        }

        private static async Task PopulatePositionTableAsync(
            SqliteConnection connection, string portfolioName, string directoryPath)
        {
            string scriptPath = Path.Join(directoryPath, portfolioName + ".sql");
            using StreamReader reader = new(scriptPath);
            await ExecuteReaderAsync(connection, reader).ConfigureAwait(false);
        }

        private static async Task ExecuteReaderAsync(SqliteConnection connection, StreamReader reader)
        {
            string commandText = await reader.ReadToEndAsync().ConfigureAwait(false);
            await using SqliteCommand command = new(commandText, connection);
            command.ExecuteReader();
        }
    }
}
