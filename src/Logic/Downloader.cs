using System;
using System.Globalization;
using System.IO;
using System.Net.Http;

namespace Diversifolio
{
    public sealed class Downloader : IDisposable
    {
        private Downloader(HttpClient client, string directory)
        {
            Client = client;
            Directory = directory;
        }

        private static CultureInfo F => CultureInfo.InvariantCulture;

        private HttpClient Client { get; }
        private string Directory { get; }

        public void Dispose() => Client.Dispose();

        internal static Downloader Create()
        {
            HttpClient httpClient = new();
            string directoryName = DateTime.Now.ToString("yyyy-MM-dd_HH", F);
            string directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), nameof(Diversifolio), directoryName);
            return new(httpClient, directory);
        }
    }
}
