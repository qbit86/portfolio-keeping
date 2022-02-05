#define DIVERSIFOLIO_TINKOFF_ENABLED

using System;
using System.IO;
#if DIVERSIFOLIO_TINKOFF_ENABLED
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

#if DIVERSIFOLIO_TINKOFF_ENABLED
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
