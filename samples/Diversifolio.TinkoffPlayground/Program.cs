using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

[assembly: CLSCompliant(true)]

namespace Diversifolio
{
    internal static class Program
    {
        private static CultureInfo F => CultureInfo.InvariantCulture;

        private static async Task Main() => await RunActualCheck().ConfigureAwait(false);

        // ReSharper disable once UnusedMember.Local
        private static async Task RunSandboxCheck()
        {
            using var reader = new StreamReader("token-tinkoff-sandbox.txt");
            string? token = await reader.ReadLineAsync().ConfigureAwait(false);
            Console.WriteLine($"{nameof(token)}: {token}");

            // https://github.com/TinkoffCreditSystems/invest-openapi-csharp-sdk
            using SandboxConnection connection = ConnectionFactory.GetSandboxConnection(token);
            SandboxContext context = connection.Context;

            // https://github.com/TinkoffCreditSystems/invest-openapi-csharp-sdk/issues/25
            SandboxAccount account = await context.RegisterAsync(BrokerAccountType.Tinkoff).ConfigureAwait(false);
            Console.WriteLine($"BrokerAccountId: {account.BrokerAccountId}");

            // https://github.com/TinkoffCreditSystems/invest-openapi-csharp-sdk/blob/master/samples/TradingBot/SandboxBot.cs
            PortfolioCurrencies portfolioCurrencies = await context.PortfolioCurrenciesAsync().ConfigureAwait(false);
            Console.WriteLine("PortfolioCurrencies:");
            foreach (PortfolioCurrencies.PortfolioCurrency currency in portfolioCurrencies.Currencies)
                Console.WriteLine($"    - {currency.Currency}, {currency.Balance.ToString(F)}");

            Portfolio portfolio = await context.PortfolioAsync().ConfigureAwait(false);
            Console.WriteLine("Portfolio:");
            foreach (Portfolio.Position position in portfolio.Positions)
            {
                MoneyAmount price = position.AveragePositionPrice;
                Console.WriteLine(
                    $"    - {position.Ticker}, {position.Balance.ToString(F)}, {price.Value.ToString(F)} {price.Currency}");
            }

            MarketInstrumentList instrumentList = await context.MarketStocksAsync().ConfigureAwait(false);
            Console.WriteLine("Instruments (EUR):");
            foreach (MarketInstrument instrument in instrumentList.Instruments)
            {
                if (instrument.Currency == Currency.Eur)
                    Console.WriteLine($"    - {instrument}");
            }
        }

        private static async Task RunActualCheck()
        {
            using var reader = new StreamReader("token-tinkoff.txt");
            string? token = await reader.ReadLineAsync().ConfigureAwait(false);
            Console.WriteLine($"{nameof(token)}: {token}");

            // https://github.com/TinkoffCreditSystems/invest-openapi-csharp-sdk
            using Connection connection = ConnectionFactory.GetConnection(token);
            Context context = connection.Context;

            // https://github.com/TinkoffCreditSystems/invest-openapi-csharp-sdk/blob/master/samples/TradingBot/SandboxBot.cs
            PortfolioCurrencies portfolioCurrencies = await context.PortfolioCurrenciesAsync().ConfigureAwait(false);
            Console.WriteLine("PortfolioCurrencies:");
            foreach (PortfolioCurrencies.PortfolioCurrency currency in portfolioCurrencies.Currencies)
                Console.WriteLine($"    - {currency.Currency}, {currency.Balance.ToString(F)}");

            Portfolio portfolio = await context.PortfolioAsync().ConfigureAwait(false);
            Console.WriteLine("Portfolio:");
            foreach (Portfolio.Position position in portfolio.Positions)
            {
                MoneyAmount price = position.AveragePositionPrice;
                Console.WriteLine(
                    $"    - {position.Ticker}, {position.Balance.ToString(F)}, {price.Value.ToString(F)} {price.Currency}");
            }
        }
    }
}
