using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Diversifolio
{
    internal static class DataHelpers
    {
        internal static async Task<SqliteConnection> CreatePortfolioConnection(
            string directoryPath, string portfolioName)
        {
            Directory.CreateDirectory(directoryPath);
            string databasePath = Path.Join(directoryPath, portfolioName + ".db");
            if (!File.Exists(databasePath))
                await CreatePortfolioDatabase(directoryPath, portfolioName, databasePath).ConfigureAwait(false);

            SqliteConnectionStringBuilder connectionStringBuilder = new()
            {
                DataSource = databasePath,
                Mode = SqliteOpenMode.ReadOnly
            };
            string connectionString = connectionStringBuilder.ToString();
            await using SqliteConnection connection = new(connectionString);
            connection.Open();
            return connection;
        }

        private static async Task CreatePortfolioDatabase(
            string directoryPath, string portfolioName, string databasePath)
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new()
            {
                DataSource = databasePath,
                Mode = SqliteOpenMode.ReadWriteCreate
            };
            string connectionString = connectionStringBuilder.ToString();
            await using SqliteConnection connection = new(connectionString);
            connection.Open();
            await CreatePositionTable(connection).ConfigureAwait(false);
            await PopulatePositionTable(connection, directoryPath, portfolioName).ConfigureAwait(false);
        }

        private static async Task CreatePositionTable(SqliteConnection connection)
        {
            var assembly = Assembly.GetExecutingAssembly();
            await using Stream stream = assembly.GetManifestResourceStream(typeof(Program), "CreatePosition.sql")!;
            using StreamReader reader = new(stream);
            await ExecuteReader(connection, reader).ConfigureAwait(false);
        }

        private static async Task PopulatePositionTable(
            SqliteConnection connection, string directoryPath, string portfolioName)
        {
            string scriptPath = Path.Join(directoryPath, portfolioName + ".sql");
            using StreamReader reader = new(scriptPath);
            await ExecuteReader(connection, reader).ConfigureAwait(false);
        }

        private static async Task ExecuteReader(SqliteConnection connection, StreamReader reader)
        {
            string commandText = await reader.ReadToEndAsync().ConfigureAwait(false);
            await using SqliteCommand command = new(commandText, connection);
            command.ExecuteReader();
        }
    }
}
