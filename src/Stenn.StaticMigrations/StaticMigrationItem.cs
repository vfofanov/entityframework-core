using Stenn.StaticMigrations.MigrationConditions;
using System;

namespace Stenn.StaticMigrations
{
    public record StaticMigrationItem<T>(string Name, T Migration, Func<StaticMigrationConditionOptions, bool>? Condition = null) : IStaticMigration
        where T : IStaticMigration
    {
        public byte[] GetHash() => Migration.GetHash();
    }
}