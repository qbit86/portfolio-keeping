namespace Diversifolio.Moex
{
    public static class Bems
    {
        private static (string, string, string)[]? s_bemTuples;

        private static (string Board, string Engine, string Market)[] BemTuples =>
            s_bemTuples ??= CreateBemTuples();

        private static (string, string, string)[] CreateBemTuples() => new[]
        {
            (Engines.Currency, Markets.Selt, Boards.Cets),
            (Engines.Stock, Markets.Bonds, Boards.Tqob),
            (Engines.Stock, Markets.Shares, Boards.Tqbr),
            (Engines.Stock, Markets.Shares, Boards.Tqtd),
            (Engines.Stock, Markets.Shares, Boards.Tqtf)
        };
    }
}
