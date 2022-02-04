using System.Collections.Generic;

namespace Stenn.EntityFrameworkCore.Data.Main.StaticMigrations.DictEntities
{
    public static class CurrencyDeclaration
    {
        public static List<Currency> GetActual()
        {
            return new List<Currency>
            {
                new() { IsoNumericCode = 1, Iso3LetterCode = "TST", DecimalDigits = 2, Description = "Test currency Changed" },
            };
        }
    }
}