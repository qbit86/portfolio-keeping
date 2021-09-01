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
        private SecurityProvider(SecurityDownloader downloader) => Downloader = downloader;

        private SecurityDownloader Downloader { get; }

        public void Dispose() => Downloader.Dispose();

        public static SecurityProvider Create()
        {
            SecurityDownloader downloader = SecurityDownloader.Create();
            return new(downloader);
        }

        public async Task<ILookup<string, Security>> GetSecuritiesByMarketLookup()
        {
            ImmutableDictionary<string, string> pathByBoard =
                await Downloader.GetPathByBoardDictionary().ConfigureAwait(false);
            throw new NotImplementedException();
        }

        private static async Task PopulateSecurities<TCollection>(
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
    }
}
