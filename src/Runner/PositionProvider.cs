using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Diversifolio
{
    internal abstract record PositionProvider(string PortfolioName)
    {
        internal string PortfolioName { get; } =
            PortfolioName ?? throw new ArgumentNullException(nameof(PortfolioName));

        internal async Task<ImmutableDictionary<string, Position>> GetPositions()
        {
            ImmutableDictionary<string, Position>.Builder builder =
                ImmutableDictionary.CreateBuilder<string, Position>(StringComparer.Ordinal);

            await UncheckedPopulatePositions(builder).ConfigureAwait(false);
            return builder.ToImmutable();
        }

        protected abstract Task UncheckedPopulatePositions(IDictionary<string, Position> positions);
    }
}
