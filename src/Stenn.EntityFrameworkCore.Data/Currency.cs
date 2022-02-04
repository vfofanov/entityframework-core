using Stenn.DictionaryEntities;

namespace Stenn.EntityFrameworkCore.Data
{
    public class Currency
    {
        public string Iso3LetterCode { get; set; }
        public int IsoNumericCode { get; set; }
        public byte DecimalDigits { get; set; }
        public string Description { get; set; }
    }
}