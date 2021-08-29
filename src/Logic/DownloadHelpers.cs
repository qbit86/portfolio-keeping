using System;
using System.Collections.Generic;
using System.Diagnostics;
using Diversifolio.Moex;

namespace Diversifolio
{
    internal static class DownloadHelpers
    {
        internal static IEnumerable<(Bem, Uri)> EnumerateUris(string market)
        {
            IEnumerable<Bem> bems = Bems.BemsByMarket[market];
            foreach (Bem bem in bems)
            {
                Debug.Assert(market == bem.Market);
                string columns = "";
                string securities = "";
                yield return (bem, Uris.BuildUri(bem.Engine, market, bem.Board, columns, securities));
            }
        }
    }
}
