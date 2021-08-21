using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

[assembly: CLSCompliant(true)]

namespace Diversifolio
{
    internal static class Program
    {
        // ReSharper disable once UnusedMember.Local
        private static TextWriter Out => Console.Out;

        private static async Task Main()
        {
            string dirName = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), nameof(Diversifolio));
            const string portfolioName = "Tinkoff";
            await using SqliteConnection _ = await DataHelpers.CreatePortfolioConnection(dirName, portfolioName)
                .ConfigureAwait(false);
        }
    }
}
