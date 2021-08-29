using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
                yield return (bem, BuildUri(bem.Engine, market, bem.Board, columns, securities));
            }
        }

        private static Uri BuildUri(string engine, string market, string board, string columns, string securities)
        {
            Uri initialUri =
                new($"https://iss.moex.com/iss/engines/{engine}/markets/{market}/boards/{board}/securities.json");
            StringBuilder queryBuilder = new(280);
            queryBuilder.Append("?iss.meta=off")
                .Append("&iss.only=securities")
                .Append($"&securities.columns={columns}")
                .Append($"&securities={securities}");
            UriBuilder uriBuilder = new(initialUri) { Query = queryBuilder.ToString() };
            return uriBuilder.Uri;
        }
    }
}
