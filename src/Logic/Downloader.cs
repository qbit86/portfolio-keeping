using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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

        public static Downloader Create()
        {
            HttpClient httpClient = new();
            string baseName = DateTime.Now.ToString("yyyy-MM-dd_HH", F);
            string directory = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), nameof(Diversifolio), baseName);
            return new(httpClient, directory);
        }

        public async Task<string> EnsureDownloaded(string board)
        {
            if (board is null)
                throw new ArgumentNullException(nameof(board));

            string baseName = board + ".json";
            string filePath = Path.Join(Directory, baseName);
            if (File.Exists(filePath))
                return filePath;

            if (!Uris.UriByBoard.TryGetValue(board, out Uri? uri))
            {
                throw new ArgumentException(
                    $"The given key '{board}' was not present in the dictionary.", nameof(board));
            }

            System.IO.Directory.CreateDirectory(Directory);

            await using Stream stream = await Client.GetStreamAsync(uri).ConfigureAwait(false);
            await using Stream destination = File.Create(filePath);
            await stream.CopyToAsync(destination).ConfigureAwait(false);
            await destination.DisposeAsync().ConfigureAwait(false);
            return filePath;
        }
    }
}
