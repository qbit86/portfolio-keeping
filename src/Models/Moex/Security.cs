using System;

namespace Diversifolio.Moex
{
    public abstract record Security(string SecId)
    {
        public string SecId { get; } = SecId ?? throw new ArgumentNullException(nameof(SecId));
    }
}
