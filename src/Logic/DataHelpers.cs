using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Diversifolio;

internal static class DataHelpers
{
    internal static async Task<SqliteConnection> CreatePortfolioReadOnlyConnectionAsync(
        string portfolioName, string populateScriptDirectory, string databaseDirectory)
    {
        Directory.CreateDirectory(databaseDirectory);
        string databasePath = Path.Join(databaseDirectory, portfolioName + ".db");
        if (!File.Exists(databasePath))
        {
            await CreatePortfolioDatabaseAsync(portfolioName, populateScriptDirectory, databasePath)
                .ConfigureAwait(false);
        }

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
        string portfolioName, string populateScriptDirectory, string databasePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using Stream stream = assembly.GetManifestResourceStream(typeof(DataHelpers), "CreatePosition.sql")!;
        using StreamReader createReader = new(stream);
        string populateScriptPath = Path.Join(populateScriptDirectory, portfolioName + ".sql");
        using StreamReader populateReader = new(populateScriptPath);

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
