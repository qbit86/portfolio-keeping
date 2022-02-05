using System;
using System.IO;
#if !DIVERSIFOLIO_TINKOFF_DISABLED
using Tinkoff.Trading.OpenApi.Models;
#endif

namespace Diversifolio;

public static class PositionProviderFactory
{
    public static PositionProvider Create(string portfolioName)
    {
        ArgumentNullException.ThrowIfNull(portfolioName);

        string databaseDirectory = Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Diversifolio));
        string populateScriptDirectory = databaseDirectory;
        return UncheckedCreate(portfolioName, populateScriptDirectory, databaseDirectory);
    }

    public static PositionProvider Create(string portfolioName, string populateScriptDirectory)
    {
        ArgumentNullException.ThrowIfNull(portfolioName);
        ArgumentNullException.ThrowIfNull(populateScriptDirectory);

        string databaseDirectory = Path.Join(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Diversifolio));
        return UncheckedCreate(portfolioName, populateScriptDirectory, databaseDirectory);
    }

    private static PositionProvider UncheckedCreate(
        string portfolioName, string populateScriptDirectory, string databaseDirectory)
    {
#if !DIVERSIFOLIO_TINKOFF_DISABLED
        return portfolioName switch
        {
            PortfolioNames.Tinkoff =>
                new TinkoffPositionProvider(portfolioName, populateScriptDirectory, databaseDirectory,
                    BrokerAccountType.Tinkoff, "token-tinkoff.txt"),
            PortfolioNames.TinkoffIis =>
                new TinkoffPositionProvider(portfolioName, populateScriptDirectory, databaseDirectory,
                    BrokerAccountType.TinkoffIis, "token-tinkoff.txt"),
            _ => new DatabasePositionProvider(portfolioName, populateScriptDirectory, databaseDirectory)
        };
#else
        return new DatabasePositionProvider(portfolioName, populateScriptDirectory, databaseDirectory);
#endif
    }
}
