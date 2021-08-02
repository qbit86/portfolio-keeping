using System;
using System.IO;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

[assembly: CLSCompliant(true)]

namespace Diversifolio
{
    internal static class Program
    {
        private static async Task Main()
        {
            // https://github.com/TinkoffCreditSystems/invest-openapi-csharp-sdk

            using var reader = new StreamReader("token-tinkoff-sandbox.txt");
            string? token = await reader.ReadLineAsync().ConfigureAwait(false);
            Console.WriteLine($"{nameof(token)}: {token}");
            using SandboxConnection connection = ConnectionFactory.GetSandboxConnection(token);
            SandboxContext context = connection.Context;
            Portfolio portfolio = await context.PortfolioAsync().ConfigureAwait(false);
            Console.WriteLine(portfolio);
        }
    }
}
