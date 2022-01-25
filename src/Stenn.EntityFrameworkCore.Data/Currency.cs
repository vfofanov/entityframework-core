using Stenn.DictionaryEntities;

namespace Stenn.EntityFrameworkCore.Data
{
    public class Currency : IDictionaryEntity<Currency>
    {
        public string Iso3LetterCode { get; set; }
        public int IsoNumericCode { get; set; }
        public byte DecimalDigits { get; set; }
        public string Description { get; set; }

        /// <inheritdoc />
        bool IDictionaryEntity<Currency>.EqualsByKey(Currency other)
        {
            return Iso3LetterCode == other.Iso3LetterCode;
        }

        /// <inheritdoc />
        bool IDictionaryEntity<Currency>.EqualsByProperties(Currency other)
        {
            return IsoNumericCode == other.IsoNumericCode &&
                   DecimalDigits == other.DecimalDigits &&
                   Description == other.Description;
        }

        /// <inheritdoc />
        void IDictionaryEntity<Currency>.CopyPropertiesFrom(Currency source)
        {
            IsoNumericCode = source.IsoNumericCode;
            DecimalDigits = source.DecimalDigits;
            Description = source.Description;
        }
    }
}