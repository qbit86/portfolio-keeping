﻿using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]

namespace Diversifolio
{
    internal static class Program
    {
        private static TextWriter Out => Console.Out;

        private static async Task Main()
        {
            const string portfolioName = PortfolioNames.Tinkoff;
            PositionProvider positionProvider = PositionProviderFactory.Create(portfolioName);
            ImmutableDictionary<string, Position> positionByTicker =
                await positionProvider.GetPositionByTickerDictionaryAsync().ConfigureAwait(false);
            var positions = positionByTicker.Values.OrderBy(it => it.Ticker, StringComparer.Ordinal).ToImmutableArray();
            foreach (Position position in positions)
                await Out.WriteLineAsync(position.ToString()).ConfigureAwait(false);

            using SecurityDownloader downloader = SecurityDownloader.Create();
            // ReSharper disable once UnusedVariable
            ImmutableDictionary<string, string> pathByBoard =
                await downloader.GetPathByBoardDictionaryAsync().ConfigureAwait(false);
        }
    }
}
