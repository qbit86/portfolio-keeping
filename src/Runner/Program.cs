using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Diversifolio.Moex;

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
            IReadOnlyDictionary<string, Position> positionByTicker =
                await positionProvider.GetPositionByTickerDictionaryAsync().ConfigureAwait(false);

            var positions = positionByTicker.Values.OrderBy(it => it.Ticker, StringComparer.Ordinal).ToImmutableArray();
            foreach (Position position in positions)
                await Out.WriteLineAsync(position.ToString()).ConfigureAwait(false);

            using var securityProvider = SecurityProvider.Create();
            ImmutableDictionary<string, ImmutableList<Security>> securitiesByMarket =
                await securityProvider.GetSecuritiesByMarketDictionaryAsync().ConfigureAwait(false);

            await Out.WriteLineAsync().ConfigureAwait(false);
            foreach (KeyValuePair<string, ImmutableList<Security>> kv in securitiesByMarket)
            {
                await Out.WriteLineAsync(kv.Key + ":").ConfigureAwait(false);
                IEnumerable<string> tickers = kv.Value.Select(it => it.SecId);
                await Out.WriteLineAsync("\t" + string.Join(", ", tickers)).ConfigureAwait(false);
            }
        }
    }
}
