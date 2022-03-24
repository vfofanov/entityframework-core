namespace Stenn.EntityFrameworkCore.Data.Initial
{
    public class CurrencyV1
    {
        public static CurrencyV1 Create(int isoNumericCode, string iso3LetterCode,  byte decimalDigits, string description)
        {
            return new CurrencyV1(iso3LetterCode, isoNumericCode, decimalDigits, description);
        }
        
        private CurrencyV1(string iso3LetterCode, int isoNumericCode, byte decimalDigits, string description)
        {
            Iso3LetterCode = iso3LetterCode;
            IsoNumericCode = isoNumericCode;
            DecimalDigits = decimalDigits;
            Description = description;
        }

        public string Iso3LetterCode { get; private set; }
        public int IsoNumericCode { get; private set; }
        public byte DecimalDigits { get; init; }
        public string Description { get; protected set; }

        public CurrencyType Type { get; set; }
    }
}