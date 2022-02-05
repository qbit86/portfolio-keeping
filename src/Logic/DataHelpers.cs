using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Diversifolio;

internal static class DataHelpers
{
    internal static async Task<SqliteConnection> CreatePortfolioReadOnlyConnectionAsync(
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
        var assembly = Assembly.GetExecutingAssembly();
        using Stream stream = assembly.GetManifestResourceStream(typeof(DataHelpers), "CreatePosition.sql")!;
        using StreamReader createReader = new(stream);
        string scriptPath = Path.Join(directoryPath, portfolioName + ".sql");
        using StreamReader populateReader = new(scriptPath);

        using SqliteConnection connection = CreateRwcConnection(databasePath);

        await ExecuteReaderAsync(connection, createReader).ConfigureAwait(false);
        await ExecuteReaderAsync(connection, populateReader).ConfigureAwait(false);

        static SqliteConnection CreateRwcConnection(string databasePath)
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new()
            {
                DataSource = databasePath,
                Mode = SqliteOpenMode.ReadWriteCreate
            };
            string connectionString = connectionStringBuilder.ToString();
            SqliteConnection connection = new(connectionString);
            connection.Open();
            return connection;
        }

        static async Task ExecuteReaderAsync(SqliteConnection connection, StreamReader reader)
        {
            string commandText = await reader.ReadToEndAsync().ConfigureAwait(false);
            using SqliteCommand command = new(commandText, connection);
            command.ExecuteReader();
        }
    }
}
