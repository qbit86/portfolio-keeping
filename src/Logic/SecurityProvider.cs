using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Diversifolio.Moex;

namespace Diversifolio
{
    public sealed class SecurityProvider : IDisposable
    {
        private static (string, SecurityFactory)[]? s_factoryTuples;
        private static Dictionary<string, SecurityFactory>? s_factoryByMarket;

        private SecurityProvider(SecurityDownloader downloader) => Downloader = downloader;

        public static IReadOnlyDictionary<string, SecurityFactory> FactoryByMarket => s_factoryByMarket ??=
            FactoryTuples.ToDictionary(it => it.Market, it => it.Factory, StringComparer.Ordinal);

        private static (string Market, SecurityFactory Factory)[] FactoryTuples =>
            s_factoryTuples ??= CreateFactoryTuples();

        private SecurityDownloader Downloader { get; }

        public void Dispose() => Downloader.Dispose();

        public static SecurityProvider Create()
        {
            var downloader = SecurityDownloader.Create();
            return new(downloader);
        }

        public async Task<IReadOnlyDictionary<string, IReadOnlyList<Security>>> GetSecuritiesByMarketDictionaryAsync()
        {
            IReadOnlyDictionary<string, string> pathByBoard =
                await Downloader.GetPathByBoardDictionaryAsync().ConfigureAwait(false);

            List<(string Market, IReadOnlyList<Security> Securities)> tuples = new(pathByBoard.Count);
            foreach (KeyValuePair<string, string> kv in pathByBoard)
            {
                string market = Bems.BemByBoard[kv.Key].Market;
                SecurityFactory securityFactory = FactoryByMarket[market];
                IReadOnlyList<Security> securities =
                    await GetSecuritiesAsync(securityFactory, kv.Value).ConfigureAwait(false);
                tuples.Add((market, securities));
            }

            IEnumerable<IGrouping<string, IReadOnlyList<Security>>> grouped =
                tuples.GroupBy(it => it.Market, it => it.Securities);
            return grouped.ToDictionary(
                grouping => grouping.Key,
                grouping => grouping.SelectMany(it => it).ToList() as IReadOnlyList<Security>,
                StringComparer.Ordinal);
        }

        public Task<IReadOnlyDictionary<string, string>> GetPathByBoardDictionaryAsync() =>
            Downloader.GetPathByBoardDictionaryAsync();

        private static async Task<IReadOnlyList<Security>> GetSecuritiesAsync(
            SecurityFactory securityFactory, string path)
        {
            List<Security> securities = new();
            await PopulateSecuritiesAsync(securityFactory, path, securities).ConfigureAwait(false);
            return securities;
        }

        private static async Task PopulateSecuritiesAsync<TCollection>(
            SecurityFactory securityFactory, string path, TCollection securities)
            where TCollection : ICollection<Security>
        {
            using FileStream stream = File.OpenRead(path);
            using JsonDocument document = await JsonDocument.ParseAsync(stream).ConfigureAwait(false);
            JsonElement securitiesElement = document.RootElement.GetProperty("securities");
            JsonElement securitiesData = securitiesElement.GetProperty("data");
            foreach (JsonElement row in securitiesData.EnumerateArray())
            {
                if (securityFactory.TryCreate(row, out Security? security))
                    securities.Add(security);
            }
        }

        private static (string, SecurityFactory)[] CreateFactoryTuples() => new (string, SecurityFactory)[]
        {
            (Markets.Selt, SeltSecurityFactory.Instance),
            (Markets.Bonds, BondSecurityFactory.Instance),
            (Markets.Shares, ShareSecurityFactory.Instance),
            (Markets.ForeignShares, ShareSecurityFactory.Instance)
        };
    }
}
