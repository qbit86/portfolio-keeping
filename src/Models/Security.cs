using System;

namespace Diversifolio
{
    public sealed class Security
    {
        public Security(string secId, string boardId, string currencyId, int lotSize, decimal faceValue, int decimals,
            decimal prevAdmittedQuote)
        {
            SecId = secId ?? throw new ArgumentNullException(nameof(secId));
            BoardId = boardId ?? throw new ArgumentNullException(nameof(boardId));
            CurrencyId = currencyId ?? throw new ArgumentNullException(nameof(currencyId));
            LotSize = lotSize;
            FaceValue = faceValue;
            Decimals = decimals;
            PrevAdmittedQuote = prevAdmittedQuote;
        }

        public string SecId { get; }
        public string BoardId { get; }
        public string CurrencyId { get; }
        public int LotSize { get; }
        public decimal FaceValue { get; }
        public int Decimals { get; }
        public decimal PrevAdmittedQuote { get; }
    }
}
