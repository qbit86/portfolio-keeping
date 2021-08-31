using System;
using System.Linq;
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

        public Task<ILookup<string, Security>> GetSecuritiesByMarketLookup() =>
            throw new NotImplementedException();
    }
}
