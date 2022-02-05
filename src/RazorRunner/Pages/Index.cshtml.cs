﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Diversifolio.Moex;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Diversifolio.Pages;

public sealed class IndexModel : PageModel
{
    internal const string PortfolioName = PortfolioNames.Test;

    private static readonly ILookup<AssetClass, Asset> s_emptyLookup = Array.Empty<Asset>().ToLookup(GetAssetClass);
    private readonly IConfiguration _configuration;

    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    private IReadOnlyDictionary<string, IReadOnlyList<Security>> SecuritiesByMarket { get; set; } =
        ImmutableDictionary<string, IReadOnlyList<Security>>.Empty;

    internal RubCurrencyConverter CurrencyConverter { get; private set; }

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
        SecuritiesByMarket = await securityProvider.GetSecuritiesByMarketDictionaryAsync().ConfigureAwait(false);

        SeltSecurity usd = SecuritiesByMarket[Markets.Selt].OfType<SeltSecurity>().Single();
        CurrencyConverter = RubCurrencyConverter.Create(usd);

        string? populateScriptDirectory = _configuration["PopulateScriptDirectory"];
        PositionProvider positionProvider = PositionProviderFactory.Create(PortfolioName, populateScriptDirectory);
        AssetProvider<RubCurrencyConverter> assetProvider = new(SecuritiesByMarket, CurrencyConverter);

        await PlanAsync(positionProvider, assetProvider).ConfigureAwait(false);
    }

    private async Task PlanAsync(PositionProvider positionProvider, AssetProvider<RubCurrencyConverter> assetProvider)
    {
        IReadOnlyList<Position> portfolioPositions =
            await positionProvider.GetPositionsAsync().ConfigureAwait(false);

        IReadOnlyList<Asset> portfolioAssets = assetProvider.GetAssets(portfolioPositions);
        PortfolioAssetsByClass = portfolioAssets.ToLookup(GetAssetClass);

        Position[] executedPositions = Array.Empty<Position>();
        IReadOnlyList<Asset> executedAssets = assetProvider.GetAssets(executedPositions);
        ExecutedAssetsByClass = executedAssets.ToLookup(GetAssetClass);

        Position[] plannedPositions = Array.Empty<Position>();
        IReadOnlyList<Asset> plannedAssets = assetProvider.GetAssets(plannedPositions);
        PlannedAssetsByClass = plannedAssets.ToLookup(GetAssetClass);

        var contributions = plannedAssets.Concat(executedAssets).ToList();
        ContributionsByClass = contributions.ToLookup(GetAssetClass);

        var mergedAssets = portfolioAssets.Concat(contributions).ToList();
        MergedAssetsByClass = mergedAssets.ToLookup(GetAssetClass);
    }

    private static AssetClass GetAssetClass(Asset asset) =>
        asset.AssetClass == AssetClass.Stock ? AssetClass.Stock : AssetClass.Other;
}
