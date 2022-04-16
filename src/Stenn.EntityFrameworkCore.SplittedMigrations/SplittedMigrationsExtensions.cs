using System;
using System.Reflection;

namespace Stenn.EntityFrameworkCore.SplittedMigrations
{
    public static class SplittedMigrationsExtensions
    {
        public static bool HasSplittedMigrations(this TypeInfo migration)
        {
            return migration.GetCustomAttribute<SplittedMigrationAttribute>() is not null;
        }

        public static SplittedMigrationAttribute GetSplittedMigrations(this TypeInfo migration)
        {
            return migration.GetCustomAttribute<SplittedMigrationAttribute>() ?? throw new InvalidOperationException();
        }
        
        public static InitialMigrationAttribute GetInitialMigration(this TypeInfo migration)
        {
            return migration.GetCustomAttribute<InitialMigrationAttribute>() ?? throw new InvalidOperationException();
        }
    }
}