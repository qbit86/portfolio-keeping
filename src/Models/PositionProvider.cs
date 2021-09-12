using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Diversifolio
{
    public abstract record PositionProvider(string PortfolioName)
    {
        public string PortfolioName { get; } =
            PortfolioName ?? throw new ArgumentNullException(nameof(PortfolioName));

        public async Task<IReadOnlyList<Position>> GetPositionsAsync()
        {
            List<Position> positions = new();
            await PopulatePositionsAsync(positions).ConfigureAwait(false);
            return positions;
        }

        protected abstract Task PopulatePositionsAsync<TCollection>(TCollection positions)
            where TCollection : ICollection<Position>;
    }
}
