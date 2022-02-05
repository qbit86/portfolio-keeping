using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Diversifolio;

public sealed record DatabasePositionProvider
    (string PortfolioName, string PopulateScriptDirectory, string DatabaseDirectory) : PositionProvider(PortfolioName)
{
    protected override async Task PopulatePositionsAsync<TCollection>(TCollection positions)
    {
        if (positions is null)
            throw new ArgumentNullException(nameof(positions));

        using SqliteConnection connection = await DataHelpers.CreatePortfolioReadOnlyConnectionAsync(
            PortfolioName, PopulateScriptDirectory, DatabaseDirectory).ConfigureAwait(false);

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
