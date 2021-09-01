using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            SecurityDownloader downloader = SecurityDownloader.Create();
            return new(downloader);
        }

        public async Task<ILookup<string, Security>> GetSecuritiesByMarketLookupAsync()
        {
            ImmutableDictionary<string, string> pathByBoard =
                await Downloader.GetPathByBoardDictionaryAsync().ConfigureAwait(false);
            throw new NotImplementedException();
        }

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
            await using FileStream stream = File.OpenRead(path);
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
            (Markets.Shares, ShareSecurityFactory.Instance)
        };
    }
}
