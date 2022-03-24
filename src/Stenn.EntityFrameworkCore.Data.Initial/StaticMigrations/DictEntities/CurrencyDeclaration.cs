using System.Collections.Generic;

namespace Stenn.EntityFrameworkCore.Data.Initial.StaticMigrations.DictEntities
{
    public static class CurrencyDeclaration
    {
        public static List<CurrencyV1> GetActual()
        {
            return new List<CurrencyV1>
            {
                CurrencyV1.Create(1, "TST", 2, "Test currency"),
                CurrencyV1.Create(2, "TS2", 2, "Test currency 2")
            };
        }
    }
}