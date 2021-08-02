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
            using var reader = new StreamReader("token-tinkoff-sandbox.txt");
            string? token = await reader.ReadLineAsync().ConfigureAwait(false);
            Console.WriteLine($"{nameof(token)}: {token}");

            // https://github.com/TinkoffCreditSystems/invest-openapi-csharp-sdk
            using SandboxConnection connection = ConnectionFactory.GetSandboxConnection(token);
            SandboxContext context = connection.Context;

            // https://github.com/TinkoffCreditSystems/invest-openapi-csharp-sdk/issues/25
            SandboxAccount _ = await context.RegisterAsync(BrokerAccountType.Tinkoff).ConfigureAwait(false);

            Portfolio portfolio = await context.PortfolioAsync().ConfigureAwait(false);
            Console.WriteLine(portfolio);
        }
    }
}
