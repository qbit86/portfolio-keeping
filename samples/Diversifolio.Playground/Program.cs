using System;
using System.IO;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]

namespace Diversifolio
{
    internal static class Program
    {
        private static async Task Main()
        {
            using var reader = new StreamReader("token-tinkoff-sandbox.txt");
            string? token = await reader.ReadLineAsync().ConfigureAwait(false);
            Console.WriteLine($"{nameof(token)}: {token}");
        }
    }
}
