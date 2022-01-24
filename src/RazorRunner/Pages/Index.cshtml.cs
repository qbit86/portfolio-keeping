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

    private readonly ILogger<IndexModel> _logger;
    private ILookup<AssetClass, Asset> _portfolioAssetsByClass = Array.Empty<Asset>().ToLookup(GetAssetClass);
    private ILookup<AssetClass, Asset> _executedAssetsByClass = Array.Empty<Asset>().ToLookup(GetAssetClass);
    private ILookup<AssetClass, Asset> _plannedAssetsByClass = Array.Empty<Asset>().ToLookup(GetAssetClass);
    private ILookup<AssetClass, Asset> _contributionsByClass = Array.Empty<Asset>().ToLookup(GetAssetClass);
    private ILookup<AssetClass, Asset> _mergedAssetsByClass = Array.Empty<Asset>().ToLookup(GetAssetClass);

    public IndexModel(ILogger<IndexModel> logger) => _logger = logger;

    internal ILookup<AssetClass, Asset> PortfolioAssetsByClass => _portfolioAssetsByClass;

    internal ILookup<AssetClass, Asset> ExecutedAssetsByClass => _executedAssetsByClass;

    internal ILookup<AssetClass, Asset> PlannedAssetsByClass => _plannedAssetsByClass;

    internal ILookup<AssetClass, Asset> ContributionsByClass => _contributionsByClass;

    internal ILookup<AssetClass, Asset> MergedAssetsByClass => _mergedAssetsByClass;

    internal async Task OnGet()
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
        _portfolioAssetsByClass = portfolioAssets.ToLookup(GetAssetClass);

#if false
        await Out.WriteLineAsync($"{nameof(portfolioAssets)} ({portfolioName})").ConfigureAwait(false);
        await assetWriter.WriteAsync(portfolioAssetsByClass).ConfigureAwait(false);

        await Out.WriteLineAsync().ConfigureAwait(false);
        await proportionWriter.WriteAsync(portfolioAssetsByClass).ConfigureAwait(false);
#endif

        Position[] executedPositions = Array.Empty<Position>();
        IReadOnlyList<Asset> executedAssets = assetProvider.GetAssets(executedPositions);
        _executedAssetsByClass = executedAssets.ToLookup(GetAssetClass);
#if false
        if (_executedAssetsByClass.Count > 0)
        {
            await Out.WriteLineAsync($"{Environment.NewLine}{nameof(executedAssets)} ({portfolioName})")
                .ConfigureAwait(false);
            await assetWriter.WriteAsync(executedAssetsByClass).ConfigureAwait(false);
        }
#endif

        Position[] plannedPositions = Array.Empty<Position>();
        IReadOnlyList<Asset> plannedAssets = assetProvider.GetAssets(plannedPositions);
        _plannedAssetsByClass = plannedAssets.ToLookup(GetAssetClass);
#if false
        if (_plannedAssetsByClass.Count > 0)
        {
            await Out.WriteLineAsync($"{Environment.NewLine}{nameof(plannedAssets)} ({portfolioName})")
                .ConfigureAwait(false);
            await assetWriter.WriteAsync(plannedAssetsByClass).ConfigureAwait(false);
        }
#endif

        var contributions = plannedAssets.Concat(executedAssets).ToList();
        _contributionsByClass = contributions.ToLookup(GetAssetClass);
#if false
        if (_contributionsByClass.Count > 0)
        {
            await Out.WriteLineAsync($"{Environment.NewLine}{nameof(contributions)} ({portfolioName})")
                .ConfigureAwait(false);
            await proportionWriter.WriteAsync(contributionsByClass).ConfigureAwait(false);
        }
#endif

        var mergedAssets = portfolioAssets.Concat(contributions).ToList();
        _mergedAssetsByClass = mergedAssets.ToLookup(GetAssetClass);
#if false
        if (_mergedAssetsByClass.Count > 0)
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
