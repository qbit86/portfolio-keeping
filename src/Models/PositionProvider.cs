using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Diversifolio
{
    public abstract record PositionProvider(string PortfolioName)
    {
        public string PortfolioName { get; } =
            PortfolioName ?? throw new ArgumentNullException(nameof(PortfolioName));

        public async Task<ImmutableDictionary<string, Position>> GetPositionByTickerDictionary()
        {
            ImmutableDictionary<string, Position>.Builder builder =
                ImmutableDictionary.CreateBuilder<string, Position>(StringComparer.Ordinal);

            await PopulatePositions(builder).ConfigureAwait(false);
            return builder.ToImmutable();
        }

        protected abstract Task PopulatePositions<TDictionary>(TDictionary positions)
            where TDictionary : IDictionary<string, Position>;
    }
}
