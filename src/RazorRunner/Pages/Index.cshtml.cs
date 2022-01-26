using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Diversifolio.Moex;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Diversifolio.Pages;

public sealed class IndexModel : PageModel
{
    internal const string PortfolioName = PortfolioNames.TinkoffIis;

    private static readonly ILookup<AssetClass, Asset> s_emptyLookup = Array.Empty<Asset>().ToLookup(GetAssetClass);

    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger) => _logger = logger;

    internal ILookup<AssetClass, Asset> PortfolioAssetsByClass { get; private set; } = s_emptyLookup;

    internal ILookup<AssetClass, Asset> ExecutedAssetsByClass { get; private set; } = s_emptyLookup;

    internal ILookup<AssetClass, Asset> PlannedAssetsByClass { get; private set; } = s_emptyLookup;

    internal ILookup<AssetClass, Asset> ContributionsByClass { get; private set; } = s_emptyLookup;

    internal ILookup<AssetClass, Asset> MergedAssetsByClass { get; private set; } = s_emptyLookup;

    internal static string GetPriceFormat(int decimalCount) =>
        decimalCount switch
        {
            0 => "F0",
            1 => "F1",
            2 => "F2",
            3 => "F3",
            4 => "F4",
            _ => "F" + decimalCount.ToString(FormattingHelpers.FormatProvider)
        };

    public async Task OnGet()
    {
        using var securityProvider = SecurityProvider.Create();
        IReadOnlyDictionary<string, IReadOnlyList<Security>> securitiesByMarket =
            await securityProvider.GetSecuritiesByMarketDictionaryAsync().ConfigureAwait(false);

        SeltSecurity usd = securitiesByMarket[Markets.Selt].OfType<SeltSecurity>().Single();
        var currencyConverter = RubCurrencyConverter.Create(usd);

        PositionProvider positionProvider = PositionProviderFactory.Create(PortfolioName);
        AssetProvider<RubCurrencyConverter> assetProvider = new(securitiesByMarket, currencyConverter);

        await PlanAsync(positionProvider, assetProvider).ConfigureAwait(false);
    }

    private async Task PlanAsync(PositionProvider positionProvider, AssetProvider<RubCurrencyConverter> assetProvider)
    {
        IReadOnlyList<Position> portfolioPositions =
            await positionProvider.GetPositionsAsync().ConfigureAwait(false);

        IReadOnlyList<Asset> portfolioAssets = assetProvider.GetAssets(portfolioPositions);
        PortfolioAssetsByClass = portfolioAssets.ToLookup(GetAssetClass);

#if false
        await Out.WriteLineAsync($"{nameof(portfolioAssets)} ({portfolioName})").ConfigureAwait(false);
        await assetWriter.WriteAsync(portfolioAssetsByClass).ConfigureAwait(false);

        await Out.WriteLineAsync().ConfigureAwait(false);
        await proportionWriter.WriteAsync(portfolioAssetsByClass).ConfigureAwait(false);
#endif

        Position[] executedPositions = Array.Empty<Position>();
        IReadOnlyList<Asset> executedAssets = assetProvider.GetAssets(executedPositions);
        ExecutedAssetsByClass = executedAssets.ToLookup(GetAssetClass);
#if false
        if (ExecutedAssetsByClass.Count > 0)
        {
            await Out.WriteLineAsync($"{Environment.NewLine}{nameof(executedAssets)} ({portfolioName})")
                .ConfigureAwait(false);
            await assetWriter.WriteAsync(executedAssetsByClass).ConfigureAwait(false);
        }
#endif

        Position[] plannedPositions = Array.Empty<Position>();
        IReadOnlyList<Asset> plannedAssets = assetProvider.GetAssets(plannedPositions);
        PlannedAssetsByClass = plannedAssets.ToLookup(GetAssetClass);
#if false
        if (PlannedAssetsByClass.Count > 0)
        {
            await Out.WriteLineAsync($"{Environment.NewLine}{nameof(plannedAssets)} ({portfolioName})")
                .ConfigureAwait(false);
            await assetWriter.WriteAsync(plannedAssetsByClass).ConfigureAwait(false);
        }
#endif

        var contributions = plannedAssets.Concat(executedAssets).ToList();
        ContributionsByClass = contributions.ToLookup(GetAssetClass);
#if false
        if (ContributionsByClass.Count > 0)
        {
            await Out.WriteLineAsync($"{Environment.NewLine}{nameof(contributions)} ({portfolioName})")
                .ConfigureAwait(false);
            await proportionWriter.WriteAsync(contributionsByClass).ConfigureAwait(false);
        }
#endif

        var mergedAssets = portfolioAssets.Concat(contributions).ToList();
        MergedAssetsByClass = mergedAssets.ToLookup(GetAssetClass);
#if false
        if (MergedAssetsByClass.Count > 0)
        {
            await Out.WriteLineAsync($"{Environment.NewLine}{nameof(mergedAssets)} ({portfolioName})")
                .ConfigureAwait(false);
            await proportionWriter.WriteAsync(mergedAssetsByClass).ConfigureAwait(false);
        }
#endif
    }

    private static AssetClass GetAssetClass(Asset asset) =>
        asset.AssetClass == AssetClass.Stock ? AssetClass.Stock : AssetClass.Other;
}
