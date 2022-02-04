using System.Collections.Generic;

namespace Stenn.EntityFrameworkCore.Data.Initial.StaticMigrations.DictEntities
{
    public static class CurrencyDeclaration
    {
        public static List<Currency> GetActual()
        {
            return new List<Currency>
            {
                new() { IsoNumericCode = 1, Iso3LetterCode = "TST", DecimalDigits = 2, Description = "Test currency" },
                new() { IsoNumericCode = 2, Iso3LetterCode = "TS2", DecimalDigits = 2, Description = "Test currency 2" }
            };
        }
    }
}