using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

namespace Diversifolio;

public sealed record TinkoffPositionProvider(
    string PortfolioName, string PopulateScriptDirectory, string DatabaseDirectory,
    BrokerAccountType BrokerAccountType, string TokenPath) :
    PositionProvider(PortfolioName)
{
    private static CultureInfo P => CultureInfo.InvariantCulture;

    private string TokenPath { get; } = TokenPath ?? throw new ArgumentNullException(nameof(TokenPath));

    protected override async Task PopulatePositionsAsync<TCollection>(TCollection positions)
    {
        if (positions is null)
            throw new ArgumentNullException(nameof(positions));

        using SqliteConnection connection =
            await CreatePortfolioConnectionAsync().ConfigureAwait(false);

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

    private async Task<SqliteConnection> CreatePortfolioConnectionAsync()
    {
        Directory.CreateDirectory(DatabaseDirectory);
        string databasePath = Path.Join(DatabaseDirectory, PortfolioName + ".db");
        if (!File.Exists(databasePath))
            await CreatePortfolioDatabaseAsync(databasePath).ConfigureAwait(false);

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

    private async Task CreatePortfolioDatabaseAsync(string databasePath)
    {
        List<Position> positions = new();
        await PopulatePositionsFromTinkoffAsync(positions).ConfigureAwait(false);
        await WriteInsertScriptAsync(positions).ConfigureAwait(false);
        await DataHelpers.CreatePortfolioDatabaseAsync(PortfolioName, PopulateScriptDirectory, databasePath)
            .ConfigureAwait(false);
    }

    private async Task PopulatePositionsFromTinkoffAsync<TCollection>(TCollection positions)
        where TCollection : ICollection<Position>
    {
        using var reader = new StreamReader(TokenPath);
        string? token = await reader.ReadLineAsync().ConfigureAwait(false);

        // https://github.com/TinkoffCreditSystems/invest-openapi-csharp-sdk
        using Connection connection = ConnectionFactory.GetConnection(token);
        Context context = connection.Context;

        IReadOnlyCollection<Account> accounts = await context.AccountsAsync().ConfigureAwait(false);
        Account account = accounts.Single(it => it.BrokerAccountType == BrokerAccountType);
        Portfolio portfolio = await context.PortfolioAsync(account.BrokerAccountId).ConfigureAwait(false);
        foreach (Portfolio.Position position in portfolio.Positions)
            positions.Add(new(Convert(position.Ticker), position.Balance));

        static string Convert(string ticker)
        {
            return ticker switch { "TSLA" => "TSLA-RM", _ => ticker };
        }
    }

    private async Task WriteInsertScriptAsync(List<Position> positions)
    {
        positions.Sort((left, right) => StringComparer.Ordinal.Compare(left.Ticker, right.Ticker));
        string populateScriptPath = Path.Join(PopulateScriptDirectory, PortfolioName + ".sql");
        using var fileStream = new FileStream(populateScriptPath, FileMode.Create, FileAccess.Write, FileShare.None);
        using var writer = new StreamWriter(fileStream, Encoding.UTF8);
        await writer.WriteLineAsync("INSERT INTO Position (Ticker, Balance)").ConfigureAwait(false);
        for (int i = 0; i < positions.Count; ++i)
        {
            Position position = positions[i];
            if (position.Ticker.Contains('\'', StringComparison.Ordinal))
                throw new InvalidOperationException("The ticker must not contain the apostrophe.");
            bool isConsequent = i > 0;
            string delimiter = isConsequent ? ",\r\n       " : "VALUES ";
            await writer.WriteAsync(delimiter).ConfigureAwait(false);
            string value = string.Create(P, $"('{position.Ticker}', {Convert.ToInt32(Math.Floor(position.Balance))})");
            await writer.WriteAsync(value).ConfigureAwait(false);
        }

        await writer.WriteLineAsync(";").ConfigureAwait(false);
    }
}
