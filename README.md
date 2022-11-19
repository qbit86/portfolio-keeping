# Diversifolio

Diversifolio is a tool for personal portfolio keeping and is not meant to be used by a general audience.

It loads free market data from Moscow Exchange [Informational & Statistical Server].
It reads portfolio positions from SQLite database.
For Tinkoff the data can be fetched via legacy [OpenAPI]; newer [Invest API] is yet to be supported.

![Screenshot](/doc/images/screenshot.png)

## How to use

Add `PopulateScriptDirectory` value to the config. 
For example, `src/RazorRunner/appsettings.Development.json`:
```json
{
  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "PopulateScriptDirectory": "~/…/portfolio-data/scripts/"
}
```

[Informational & Statistical Server]: https://moex.com/a2920
[OpenAPI]: https://github.com/Tinkoff/invest-openapi-csharp-sdk
[Invest API]: https://github.com/Tinkoff/invest-api-csharp-sdk
