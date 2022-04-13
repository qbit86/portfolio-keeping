using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Tinkoff.Trading.OpenApi.Models;
using Tinkoff.Trading.OpenApi.Network;

[assembly: CLSCompliant(true)]

namespace Diversifolio;

internal static class Program
{
    private static CultureInfo P => CultureInfo.InvariantCulture;

    private static async Task Main() => await RunActualCheckAsync().ConfigureAwait(false);

    // ReSharper disable once UnusedMember.Local
    private static async Task RunSandboxCheckAsync()
    {
        using var reader = new StreamReader("token-tinkoff-sandbox.txt");
        string? token = await reader.ReadLineAsync().ConfigureAwait(false);
        Console.WriteLine($"{nameof(token)}: {token}");

        // https://github.com/Tinkoff/invest-openapi-csharp-sdk
        using SandboxConnection connection = ConnectionFactory.GetSandboxConnection(token);
        SandboxContext context = connection.Context;

        // https://github.com/TinkoffCreditSystems/invest-openapi-csharp-sdk/issues/25
        SandboxAccount account = await context.RegisterAsync(BrokerAccountType.Tinkoff).ConfigureAwait(false);
        Console.WriteLine($"BrokerAccountId: {account.BrokerAccountId}");

        // https://github.com/TinkoffCreditSystems/invest-openapi-csharp-sdk/blob/master/samples/TradingBot/SandboxBot.cs
        PortfolioCurrencies portfolioCurrencies = await context.PortfolioCurrenciesAsync().ConfigureAwait(false);
        Console.WriteLine("PortfolioCurrencies:");
        foreach (PortfolioCurrencies.PortfolioCurrency currency in portfolioCurrencies.Currencies)
            WriteLine(P, $"    - {currency.Currency}, {currency.Balance}");

        Portfolio portfolio = await context.PortfolioAsync().ConfigureAwait(false);
        Console.WriteLine("Portfolio:");
        foreach (Portfolio.Position position in portfolio.Positions)
        {
            MoneyAmount price = position.AveragePositionPrice;
            WriteLine(P, $"    - {position.Ticker}, {position.Balance}, {price.Value} {price.Currency}");
        }

        MarketInstrumentList instrumentList = await context.MarketStocksAsync().ConfigureAwait(false);
        Console.WriteLine("Instruments (EUR):");
        foreach (MarketInstrument instrument in instrumentList.Instruments)
        {
            if (instrument.Currency == Currency.Eur)
                Console.WriteLine($"    - {instrument}");
        }
    }

    private static async Task RunActualCheckAsync()
    {
        using var reader = new StreamReader("token-tinkoff.txt");
        string? token = await reader.ReadLineAsync().ConfigureAwait(false);
        Console.WriteLine($"{nameof(token)}: {token}");

        // https://github.com/Tinkoff/invest-openapi-csharp-sdk
        using Connection connection = ConnectionFactory.GetConnection(token);
        Context context = connection.Context;

        IReadOnlyCollection<Account> accounts = await context.AccountsAsync().ConfigureAwait(false);
        Console.WriteLine("Accounts:");
        foreach (Account account in accounts)
        {
            BrokerAccountType brokerAccountType = account.BrokerAccountType;
            string brokerAccountId = account.BrokerAccountId;
            Console.WriteLine($"\t- {brokerAccountType},\t{brokerAccountId}");

            // https://github.com/TinkoffCreditSystems/invest-openapi-csharp-sdk/blob/master/samples/TradingBot/SandboxBot.cs
            PortfolioCurrencies portfolioCurrencies =
                await context.PortfolioCurrenciesAsync(brokerAccountId).ConfigureAwait(false);
            Console.WriteLine($"\t\t- PortfolioCurrencies ({brokerAccountType}):");
            foreach (PortfolioCurrencies.PortfolioCurrency currency in portfolioCurrencies.Currencies)
                WriteLine(P, $"\t\t\t- {currency.Currency},\t{currency.Balance}");

            Portfolio portfolio = await context.PortfolioAsync(brokerAccountId).ConfigureAwait(false);
            Console.WriteLine($"\t\t- Portfolio ({brokerAccountType}):");
            foreach (Portfolio.Position position in portfolio.Positions)
            {
                MoneyAmount price = position.AveragePositionPrice;
                WriteLine(P, $"\t\t\t- {position.Ticker},\t{position.Balance},\t{price.Value}\t{price.Currency}");
            }
        }
    }

    private static void WriteLine(IFormatProvider provider,
        [InterpolatedStringHandlerArgument("provider")] ref DefaultInterpolatedStringHandler handler) =>
        Console.WriteLine(handler.ToStringAndClear());
}
