using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Diversifolio.Moex;

namespace Diversifolio
{
    public sealed class SecurityDownloader : IDisposable
    {
        private SecurityDownloader(HttpClient client, string directory)
        {
            Client = client;
            Directory = directory;
        }

        private static CultureInfo F => CultureInfo.InvariantCulture;

        private HttpClient Client { get; }

        private string Directory { get; }

        public void Dispose() => Client.Dispose();

        public static SecurityDownloader Create()
        {
            HttpClient httpClient = new();
            string baseName = DateTime.Now.ToString("yyyy-MM-dd_HH", F);
            string directory = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Diversifolio), baseName);
            return new(httpClient, directory);
        }

        public async Task<IReadOnlyDictionary<string, string>> GetPathByBoardDictionaryAsync()
        {
            Dictionary<string, string> pathByBoard = new(StringComparer.Ordinal);
            foreach (string board in Bems.BemByBoard.Keys)
            {
                string path = await EnsureDownloadedAsync(board).ConfigureAwait(false);
                pathByBoard[board] = path;
            }

            return pathByBoard;
        }

        private async Task<string> EnsureDownloadedAsync(string board)
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

            using Stream source = await Client.GetStreamAsync(uri).ConfigureAwait(false);
            using FileStream destination = File.Create(filePath);
            await source.CopyToAsync(destination).ConfigureAwait(false);
            await destination.DisposeAsync().ConfigureAwait(false);
            return filePath;
        }
    }
}
