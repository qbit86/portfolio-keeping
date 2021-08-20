using System;
using System.IO;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]

namespace Diversifolio
{
    internal static class Program
    {
        private static TextWriter Out => Console.Out;

        private static async Task Main()
        {
            string dirName = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Diversifolio));
            Directory.CreateDirectory(dirName);
            const string baseName = "Tinkoff.sql";
            string path = Path.Join(dirName, baseName);
            await Out.WriteLineAsync($"{nameof(path)}: {path}").ConfigureAwait(false);
        }
    }
}
