using Stenn.StaticMigrations.MigrationConditions;
using System;
using System.Collections.Immutable;

namespace Stenn.StaticMigrations
{
    public record StaticMigrationItem<T>(string Name, T Migration, IImmutableSet<string> Tags, Func<StaticMigrationConditionOptions, bool>? Condition = null) : IStaticMigration
        where T : IStaticMigration
    {
        public bool IsAction() => Condition != null;

        public bool CanRun(StaticMigrationConditionOptions options) => Condition?.Invoke(options) ?? true;

        public byte[] GetHash() => Migration.GetHash();
    }
}