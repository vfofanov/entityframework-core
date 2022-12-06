using Stenn.EntityFrameworkCore.Data.Main.HistoricalWithoutAttribute;
using System.Collections.Generic;

namespace Stenn.EntityFrameworkCore.Data.Main.HistoricalWithoutAttribute.StaticMigrations.DictEntities
{
    public static class CurrencyDeclaration
    {
        public static List<Currency> GetActual()
        {
            return new List<Currency>
            {
                Currency.Create(1, "TST", 1, "Test currency Changed", CurrencyType.Cripto)
            };
        }
    }
}