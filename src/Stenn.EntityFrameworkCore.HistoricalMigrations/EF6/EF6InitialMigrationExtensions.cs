using System;
using System.Reflection;

namespace Stenn.EntityFrameworkCore.HistoricalMigrations.EF6
{
    public static class EF6InitialMigrationExtensions
    {
        public static bool HasEF6InitialMigrationAttribute(this TypeInfo migration)
        {
            return migration.GetCustomAttribute<EF6InitialMigrationAttribute>() is not null;
        }

        public static EF6InitialMigrationAttribute GetEF6InitialMigrationAttribute(this TypeInfo migration)
        {
            return migration.GetCustomAttribute<EF6InitialMigrationAttribute>() ?? 
                   throw new InvalidOperationException();
        }
    }
}