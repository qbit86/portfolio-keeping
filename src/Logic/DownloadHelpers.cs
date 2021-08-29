using System;
using System.Text;

namespace Diversifolio
{
    internal static class DownloadHelpers
    {
        internal static Uri BuildUri(string engine, string market, string board, string columns, string securities)
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
