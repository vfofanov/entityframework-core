using System;
using System.Reflection;

namespace Stenn.EntityFrameworkCore.HistoricalMigrations
{
    public static class HistoricalMigrationsExtensions
    {
        public static bool HasHistoricalMigrations(this TypeInfo migration)
        {
            return migration.GetCustomAttribute<HistoricalMigrationAttribute>() is not null;
        }

        public static HistoricalMigrationAttribute GetHistoricalMigrations(this TypeInfo migration)
        {
            return migration.GetCustomAttribute<HistoricalMigrationAttribute>() ?? throw new InvalidOperationException();
        }
        
        public static InitialMigrationAttribute GetInitialMigration(this TypeInfo migration)
        {
            return migration.GetCustomAttribute<InitialMigrationAttribute>() ?? throw new InvalidOperationException();
        }
    }
}