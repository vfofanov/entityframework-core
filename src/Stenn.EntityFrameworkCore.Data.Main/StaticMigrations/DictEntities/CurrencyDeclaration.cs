using System.Collections.Generic;

namespace Stenn.EntityFrameworkCore.Data.Main.StaticMigrations.DictEntities
{
    public static class CurrencyDeclaration
    {
        public static List<Currency> GetActual()
        {
            return new List<Currency>
            {
                Currency.Create(1, "TST", 1, "Test currency Changed")
            };
        }
    }
}