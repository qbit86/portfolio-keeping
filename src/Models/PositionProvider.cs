using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Diversifolio
{
    public abstract record PositionProvider(string PortfolioName)
    {
        public string PortfolioName { get; } =
            PortfolioName ?? throw new ArgumentNullException(nameof(PortfolioName));

        public async Task<IReadOnlyDictionary<string, Position>> GetPositionByTickerDictionaryAsync()
        {
            Dictionary<string, Position> positionByTicker = new(StringComparer.Ordinal);
            await PopulatePositionsAsync(positionByTicker).ConfigureAwait(false);
            return positionByTicker;
        }

        protected abstract Task PopulatePositionsAsync<TDictionary>(TDictionary positions)
            where TDictionary : IDictionary<string, Position>;
    }
}
