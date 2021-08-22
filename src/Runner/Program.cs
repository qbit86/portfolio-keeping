using System;
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
            const string portfolioName = PortfolioNames.TinkoffIis;
            PositionProvider positionProvider = PositionProviderFactory.Create(portfolioName);
            ImmutableDictionary<string, Position> positionByTicker =
                await positionProvider.GetPositions().ConfigureAwait(false);
            var positions = positionByTicker.Values.OrderBy(it => it.Ticker, StringComparer.Ordinal).ToImmutableArray();
            foreach (Position position in positions)
                await Out.WriteLineAsync(position.ToString()).ConfigureAwait(false);
        }
    }
}
