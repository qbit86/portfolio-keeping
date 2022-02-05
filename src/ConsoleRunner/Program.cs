using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Diversifolio.Moex;
using Microsoft.Extensions.Configuration;

[assembly: CLSCompliant(true)]

namespace Diversifolio;

internal static class Program
{
    private static TextWriter Out => Console.Out;

    private static async Task Main()
    {
        const string portfolioName = PortfolioNames.Test;

        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        using var securityProvider = SecurityProvider.Create();
        IReadOnlyDictionary<string, IReadOnlyList<Security>> securitiesByMarket =
            await securityProvider.GetSecuritiesByMarketDictionaryAsync().ConfigureAwait(false);

        SeltSecurity usd = securitiesByMarket[Markets.Selt].OfType<SeltSecurity>().Single();
        var currencyConverter = RubCurrencyConverter.Create(usd);

        if (config["PopulateScriptDirectory"] is not { } populateScriptDirectory)
            throw new InvalidOperationException("Value must not be null: " + nameof(populateScriptDirectory));

        PositionProvider positionProvider = PositionProviderFactory.Create(portfolioName, populateScriptDirectory);
        AssetProvider<RubCurrencyConverter> assetProvider = new(securitiesByMarket, currencyConverter);

        AssetWriter assetWriter = new(Out);
        var proportionWriter = ProportionWriter.Create(currencyConverter, Out);

        await PlanAsync(portfolioName, positionProvider, assetProvider, assetWriter, proportionWriter)
            .ConfigureAwait(false);
    }

    private static async Task PlanAsync(string portfolioName,
        PositionProvider positionProvider, AssetProvider<RubCurrencyConverter> assetProvider,
        AssetWriter assetWriter, ProportionWriter<RubCurrencyConverter> proportionWriter)
    {
        IReadOnlyList<Position> portfolioPositions =
            await positionProvider.GetPositionsAsync().ConfigureAwait(false);

        IReadOnlyList<Asset> portfolioAssets = assetProvider.GetAssets(portfolioPositions);
        ILookup<AssetClass, Asset> portfolioAssetsByClass = portfolioAssets.ToLookup(GetAssetClass);

        await Out.WriteLineAsync($"{nameof(portfolioAssets)} ({portfolioName})").ConfigureAwait(false);
        await assetWriter.WriteAsync(portfolioAssetsByClass).ConfigureAwait(false);

        await Out.WriteLineAsync().ConfigureAwait(false);
        await proportionWriter.WriteAsync(portfolioAssetsByClass).ConfigureAwait(false);

        Position[] executedPositions = Array.Empty<Position>();
        IReadOnlyList<Asset> executedAssets = assetProvider.GetAssets(executedPositions);
        ILookup<AssetClass, Asset> executedAssetsByClass = executedAssets.ToLookup(GetAssetClass);
        if (executedAssetsByClass.Count > 0)
        {
            await Out.WriteLineAsync($"{Environment.NewLine}{nameof(executedAssets)} ({portfolioName})")
                .ConfigureAwait(false);
            await assetWriter.WriteAsync(executedAssetsByClass).ConfigureAwait(false);
        }

        Position[] plannedPositions = Array.Empty<Position>();
        IReadOnlyList<Asset> plannedAssets = assetProvider.GetAssets(plannedPositions);
        ILookup<AssetClass, Asset> plannedAssetsByClass = plannedAssets.ToLookup(GetAssetClass);
        if (plannedAssetsByClass.Count > 0)
        {
            await Out.WriteLineAsync($"{Environment.NewLine}{nameof(plannedAssets)} ({portfolioName})")
                .ConfigureAwait(false);
            await assetWriter.WriteAsync(plannedAssetsByClass).ConfigureAwait(false);
        }

        var contributions = plannedAssets.Concat(executedAssets).ToList();
        ILookup<AssetClass, Asset> contributionsByClass = contributions.ToLookup(GetAssetClass);
        if (contributionsByClass.Count > 0)
        {
            await Out.WriteLineAsync($"{Environment.NewLine}{nameof(contributions)} ({portfolioName})")
                .ConfigureAwait(false);
            await proportionWriter.WriteAsync(contributionsByClass).ConfigureAwait(false);
        }

        var mergedAssets = portfolioAssets.Concat(contributions).ToList();
        ILookup<AssetClass, Asset> mergedAssetsByClass = mergedAssets.ToLookup(GetAssetClass);
        if (contributionsByClass.Count > 0)
        {
            await Out.WriteLineAsync($"{Environment.NewLine}{nameof(mergedAssets)} ({portfolioName})")
                .ConfigureAwait(false);
            await proportionWriter.WriteAsync(mergedAssetsByClass).ConfigureAwait(false);
        }
    }

    private static AssetClass GetAssetClass(Asset asset) =>
        asset.AssetClass == AssetClass.Stock ? AssetClass.Stock : AssetClass.Other;
}
